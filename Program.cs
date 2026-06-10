using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace chetv_dz
{
    class Program
    {
        private const string LogFilePath = "battle.log";
        private const string SaveFilePath = "GameSaveFile.dat";

        private static BattleStats _stats;
        private static ILogger _logger;
        private static GameClock _clock;
        private static RandomEventGenerator _eventGenerator;
        private static SaveManager _saveManager;
        private static bool _isRunning = true;

        private static bool _isCasting = false;
        private static CancellationTokenSource _castCts;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool startNew = await HandleStartupAsync();

            if (!startNew)
                return;

            await RunGameLoopAsync();
        }

        private static async Task<bool> HandleStartupAsync()
        {
            bool logExists = File.Exists(LogFilePath);
            bool saveExists = File.Exists(SaveFilePath);

            if (!logExists && !saveExists)
            {
                await StartNewGameAsync();
                return true;
            }

            Console.WriteLine("Файл сохранения найден.");
            Console.WriteLine("1 - Начать заново");
            Console.WriteLine("2 - Продолжить");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                await StartNewGameAsync();
                return true;
            }
            else if (choice == "2")
            {
                await ContinueGameAsync();
                return true;
            }
            else
            {
                Console.WriteLine("Неверный ввод. Выход.");
                return false;
            }
        }

        private static async Task StartNewGameAsync()
        {
            if (File.Exists(LogFilePath))
                File.Delete(LogFilePath);
            if (File.Exists(SaveFilePath))
                File.Delete(SaveFilePath);

            _stats = new BattleStats();
            _logger = new FileLogger(LogFilePath);
            _clock = new GameClock(_stats);
            _saveManager = new SaveManager(SaveFilePath);

            _eventGenerator = new RandomEventGenerator(_stats, _logger);
            _eventGenerator.OnEventOccurred += (msg) => Console.WriteLine($"\n[Событие] {msg}");
            _eventGenerator.OnRageEvent += () =>
            {
                if (_isCasting && _castCts != null)
                {
                    _castCts.Cancel();
                }
            };

            _clock.Start();
            _eventGenerator.Start();

            await _logger.LogAsync("GAME_START");
            await _saveManager.SaveAsync(_stats);

            Console.WriteLine("Новая игра начата!");
        }

        private static async Task ContinueGameAsync()
        {
            _logger = new FileLogger(LogFilePath);
            _saveManager = new SaveManager(SaveFilePath);

            var save = await _saveManager.LoadAsync();
            if (save == null)
            {
                Console.WriteLine("Ошибка загрузки сохранения. Начинаем новую игру.");
                await StartNewGameAsync();
                return;
            }

            _stats = new BattleStats();
            save.ApplyTo(_stats);

            _clock = new GameClock(_stats);
            _eventGenerator = new RandomEventGenerator(_stats, _logger);
            _eventGenerator.OnEventOccurred += (msg) => Console.WriteLine($"\n[Событие] {msg}");
            _eventGenerator.OnRageEvent += () =>
            {
                if (_isCasting && _castCts != null)
                {
                    _castCts.Cancel();
                }
            };

            _clock.Start();
            _eventGenerator.Start();

            var counters = await _logger.GetHistoryCountersAsync();
            Console.WriteLine($"\nИстория: {counters}\n");

            Console.WriteLine("Игра продолжена!");
        }

        private static async Task RunGameLoopAsync()
        {
            while (_isRunning && _stats.PlayerHP > 0 && _stats.EnemyHP > 0)
            {
                Console.WriteLine($"\n[HP: {_stats.PlayerHP}/100 | Противник: {_stats.EnemyHP}/300 | Тик: {_stats.Tick}]");
                Console.WriteLine("Доступные команды: attack, heal, stats, log, exit" + (!_isCasting ? ", cast" : ""));
                Console.Write("> ");

                string input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "attack":
                        await AttackAsync();
                        break;
                    case "heal":
                        await HealAsync();
                        break;
                    case "cast":
                        if (!_isCasting)
                            await CastAsync();
                        else
                            Console.WriteLine("Уже выполняется каст!");
                        break;
                    case "stats":
                        ShowStats();
                        break;
                    case "log":
                        await ShowLogAsync();
                        break;
                    case "exit":
                        await ExitGameAsync();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда.");
                        break;
                }

                await _saveManager.SaveAsync(_stats);

                if (_stats.EnemyHP <= 0)
                {
                    await EndGameAsync("Игрок");
                    return;
                }
                if (_stats.PlayerHP <= 0)
                {
                    await EndGameAsync("Противник");
                    return;
                }
            }
        }

        private static async Task AttackAsync()
        {
            Random rand = new Random();
            int damage = rand.Next(8, 16);
            _stats.EnemyHP -= damage;
            if (_stats.EnemyHP < 0) _stats.EnemyHP = 0;

            await _logger.LogAsync("ATTACK", ("dmg", damage), ("enemyHP", _stats.EnemyHP), ("playerHP", _stats.PlayerHP));
            Console.WriteLine($"⚔️ Вы нанесли {damage} урона противнику!");
        }

        private static async Task HealAsync()
        {
            Random rand = new Random();
            int heal = rand.Next(5, 13);
            _stats.PlayerHP += heal;
            if (_stats.PlayerHP > 100) _stats.PlayerHP = 100;

            await _logger.LogAsync("HEAL", ("value", heal), ("playerHP", _stats.PlayerHP));
            Console.WriteLine($"💚 Вы восстановили {heal} HP!");
        }

        private static async Task CastAsync()
        {
            _isCasting = true;
            _castCts = new CancellationTokenSource();

            await _logger.LogAsync("CAST_START", ("durationMs", 3000));

            Console.WriteLine("🔮 Начинается каст заклинания! 3 секунды...");

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(1000, _castCts.Token);
                    if (_castCts.Token.IsCancellationRequested)
                        throw new OperationCanceledException();
                    Console.Write(".");
                }
                Console.WriteLine();

                Random rand = new Random();
                int bonusDamage = rand.Next(20, 36);
                _stats.EnemyHP -= bonusDamage;
                if (_stats.EnemyHP < 0) _stats.EnemyHP = 0;

                await _logger.LogAsync("CAST_SUCCESS", ("bonusDmg", bonusDamage), ("enemyHP", _stats.EnemyHP));
                Console.WriteLine($"✨ Каст завершён! Нанесено {bonusDamage} урона заклинанием!");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n❌ Каст прерван из-за события «Ярость врага»!");
                await _logger.LogAsync("CAST_CANCELLED", ("reason", "rage"));
            }
            finally
            {
                _isCasting = false;
                _castCts.Dispose();
                _castCts = null;
            }
        }

        private static void ShowStats()
        {
            Console.WriteLine($"\n📊 СТАТИСТИКА:");
            Console.WriteLine($"   Игрок: {_stats.PlayerHP}/100 HP");
            Console.WriteLine($"   Противник: {_stats.EnemyHP}/300 HP");
            Console.WriteLine($"   Тик: {_stats.Tick}");
        }

        private static async Task ShowLogAsync()
        {
            var lines = await _logger.GetLastLinesAsync(10);
            Console.WriteLine("\n📋 Последние 10 строк лога:");
            if (lines.Length == 0)
            {
                Console.WriteLine("   (лог пуст)");
            }
            else
            {
                foreach (var line in lines)
                {
                    Console.WriteLine($"   {line}");
                }
            }
        }

        private static async Task ExitGameAsync()
        {
            Console.WriteLine("Завершение игры...");
            await _clock.StopAsync();
            if (_eventGenerator != null)
                await _eventGenerator.StopAsync();
            _logger.Shutdown();
            _isRunning = false;
        }

        private static async Task EndGameAsync(string winner)
        {
            Console.WriteLine($"\n🏆 Бой закончен. Победил: {winner}");

            var counters = await _logger.GetHistoryCountersAsync();
            Console.WriteLine($"\nИстория: {counters}");

            await _clock.StopAsync();
            if (_eventGenerator != null)
                await _eventGenerator.StopAsync();
            _logger.Shutdown();

            if (File.Exists(LogFilePath))
                File.Delete(LogFilePath);
            _saveManager.DeleteSave();

            Console.WriteLine("Файлы лога и сохранения удалены. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}