using System;
using System.Threading;
using System.Threading.Tasks;

namespace chetv_dz
{
    public class RandomEventGenerator
    {
        private readonly BattleStats _stats;
        private readonly ILogger _logger;
        private readonly Random _random = new Random();
        private CancellationTokenSource _cts;
        private Task _eventTask;

        public event Action<string> OnEventOccurred;
        public event Action OnRageEvent;

        public RandomEventGenerator(BattleStats stats, ILogger logger)
        {
            _stats = stats;
            _logger = logger;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _eventTask = RunEventsAsync(_cts.Token);
        }

        public async Task StopAsync()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                try
                {
                    await _eventTask;
                }
                catch (OperationCanceledException) { }
                _cts.Dispose();
                _cts = null;
            }
        }

        private async Task RunEventsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int delay = _random.Next(2, 6);
                await Task.Delay(delay * 1000, token);

                if (token.IsCancellationRequested) break;

                int eventType = _random.Next(0, 3);
                await ProcessEvent(eventType);
            }
        }

        private async Task ProcessEvent(int eventType)
        {
            if (eventType == 0) 
            {
                int damage = _random.Next(2, 7);
                _stats.PlayerHP -= damage;
                if (_stats.PlayerHP < 0) _stats.PlayerHP = 0;

                await _logger.LogAsync("EVENT", ("kind", "RAGE"), ("hpDelta", -damage), ("playerHP", _stats.PlayerHP));
                OnEventOccurred?.Invoke($"🔥 Ярость врага! Вы теряете {damage} HP.");
                OnRageEvent?.Invoke();
            }
            else if (eventType == 1) 
            {
                int heal = _random.Next(1, 5);
                _stats.PlayerHP += heal;
                if (_stats.PlayerHP > 100) _stats.PlayerHP = 100;

                await _logger.LogAsync("EVENT", ("kind", "BANDAGE"), ("hpDelta", heal), ("playerHP", _stats.PlayerHP));
                OnEventOccurred?.Invoke($"🩹 Вы нашли бинт! Восстановлено {heal} HP.");
            }
            else 
            {
                await _logger.LogAsync("EVENT", ("kind", "CALM"), ("hpDelta", 0), ("playerHP", _stats.PlayerHP));
                OnEventOccurred?.Invoke($"🌙 Тишина... Ничего не произошло.");
            }
        }
    }
}