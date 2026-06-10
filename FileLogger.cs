using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chetv_dz
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _isShutdown = false;

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public async Task LogAsync(string type, params (string key, object value)[] fields)
        {
            if (_isShutdown) return;

            await _semaphore.WaitAsync();
            try
            {
                var sb = new StringBuilder();
                sb.Append($"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ} | {type}");

                foreach (var field in fields)
                {
                    sb.Append($" | {field.key}={field.value}");
                }

                await Task.Run(() => File.AppendAllLines(_logFilePath, new[] { sb.ToString() }));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string[]> GetLastLinesAsync(int count)
        {
            if (!File.Exists(_logFilePath))
                return new string[0];

            await _semaphore.WaitAsync();
            try
            {
                var lines = await Task.Run(() => File.ReadAllLines(_logFilePath));
                return lines.Skip(Math.Max(0, lines.Length - count)).ToArray();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<HistoryCounters> GetHistoryCountersAsync()
        {
            var counters = new HistoryCounters();

            if (!File.Exists(_logFilePath))
                return counters;

            await _semaphore.WaitAsync();
            try
            {
                var lines = await Task.Run(() => File.ReadAllLines(_logFilePath));
                foreach (var line in lines)
                {
                    if (line.Contains("| ATTACK |"))
                        counters.Attack++;
                    else if (line.Contains("| HEAL |"))
                        counters.Heal++;
                    else if (line.Contains("| EVENT |"))
                        counters.Event++;
                    else if (line.Contains("| CAST_CANCELLED |"))
                        counters.CastCancelled++;
                    else if (line.Contains("| CAST_SUCCESS |"))
                        counters.CastSuccess++;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return counters;
        }

        public void Shutdown()
        {
            _isShutdown = true;
            _semaphore.Dispose();
        }
    }
}