using System;
using System.Threading;
using System.Threading.Tasks;

namespace chetv_dz
{
    public class GameClock
    {
        private readonly BattleStats _stats;
        private CancellationTokenSource _cts;
        private Task _clockTask;

        public event Action<int> OnTick;

        public GameClock(BattleStats stats)
        {
            _stats = stats;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _clockTask = RunClockAsync(_cts.Token);
        }

        public async Task StopAsync()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                try
                {
                    await _clockTask;
                }
                catch (OperationCanceledException) { }
                _cts.Dispose();
                _cts = null;
            }
        }

        private async Task RunClockAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);
                _stats.Tick++;
                OnTick?.Invoke(_stats.Tick);
            }
        }
    }
}