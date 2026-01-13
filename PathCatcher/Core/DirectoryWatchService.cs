using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PathCatcher.Core
{
    public sealed class DirectoryWatchService : IDisposable
    {
        private readonly Dictionary<string, FileSystemWatcher> watchers = new();

        public event EventHandler<FileSystemEventArgs> FileCreated;

        public void StartWatch(string path)
        {
            if (watchers.ContainsKey(path))
            {
                return;
            }

            var watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
            };

            watcher.Created += OnCreated;

            watchers[path] = watcher;
        }

        public void StopWatch(string path)
        {
            if (!watchers.TryGetValue(path, out var watcher))
            {
                return;
            }

            watcher.Created -= OnCreated;
            watcher.Dispose();
            watchers.Remove(path);
        }

        public void Dispose()
        {
            foreach (var watcher in watchers.Values)
            {
                watcher.Created -= OnCreated;
                watcher.Dispose();
            }

            watchers.Clear();
        }

        // ReSharper disable once AsyncVoidEventHandlerMethod
        // イベントハンドラのため、シグネチャの変更は不可。
        private static async Task<bool> WaitForFileReadyAsync(string path, TimeSpan timeout, int retryDelayMs = 200)
        {
            var sw = Stopwatch.StartNew();

            while (sw.Elapsed < timeout)
            {
                try
                {
                    await using var stream = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.None);

                    // 開けた＝誰も掴んでない
                    return true;
                }
                catch (IOException)
                {
                    // まだ書き込み中
                }
                catch (UnauthorizedAccessException)
                {
                    // 作成直後でアクセス不可なケース
                }

                await Task.Delay(retryDelayMs);
            }

            return false;
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                return;
            }

            var ready = await WaitForFileReadyAsync(e.FullPath, TimeSpan.FromSeconds(10));
            if (!ready)
            {
                return; // タイムアウト or 失敗
            }

            FileCreated?.Invoke(this, e);
        }
    }
}