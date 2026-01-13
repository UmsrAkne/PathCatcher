using System;
using System.Collections.Generic;
using System.IO;

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

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            FileCreated?.Invoke(this, e);
        }
    }
}