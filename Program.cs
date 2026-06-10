using System;

namespace third_dz
{
    class Program
    {
        private const int MaxHp = 100;
        private const int PlayerStrength = 5;

        private static int _playerHp = MaxHp;
        private static int _enemyHp = MaxHp;
        private static int _round = 1;

        private static IEnemyAttackStrategy _enemyStrategy;
        private static bool _hasSwitchedToCareful = false;
        private static bool _hasSwitchedToRandom = false;

        private static readonly LightPlayerAttackStrategy _lightStrategy = new LightPlayerAttackStrategy();
        private static readonly HeavyPlayerAttackStrategy _heavyStrategy = new HeavyPlayerAttackStrategy(12, PlayerStrength, 2);

        private static readonly PlayerDamagePipeline _pipeline = new PlayerDamagePipeline();

        static void Main(string[] args)
        {
            _pipeline.Modifiers += PlayerDamagePipeline.ApplyBonusIfEnemyLow;
            _pipeline.Modifiers += PlayerDamagePipeline.ApplyPenaltyIfPlayerLow;

            _enemyStrategy = new AggressiveEnemyStrategy();

            bool exit = false;

            while (!exit)
            {
                DisplayStateAndMenu();

                int choice = ReadIntInput(0, 4, "Ваш выбор: ");

                if (choice == 0)
                {
                    exit = true;
                    continue;
                }

                if (choice == 4)
                {
                    ShowStateOnly();
                    continue;
                }

                bool playerAlive = ProcessPlayerTurn(choice);
                if (!playerAlive) break;

                if (_enemyHp <= 0)
                {
                    Console.WriteLine("\nПоздравляем! Вы победили!\n");
                    break;
                }

                UpdateEnemyStrategy();

                bool enemyAlive = ProcessEnemyTurn();
                if (!enemyAlive) break;

                if (_playerHp <= 0)
                {
                    Console.WriteLine("\nПротивник победил! Game over.\n");
                    break;
                }

                _round++;
            }

            Console.WriteLine("Игра завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static void DisplayStateAndMenu()
        {
            string strategyName = GetEnemyStrategyName();
            Console.WriteLine($"\n--- Раунд {_round} | Игрок {_playerHp} HP | Сила {PlayerStrength} | Противник {_enemyHp} HP | Стратегия: {strategyName} ---");
            Console.WriteLine("1 - Лёгкий удар");
            Console.WriteLine("2 - Тяжёлый удар");
            Console.WriteLine("3 - Отдых");
            Console.WriteLine("4 - Показать состояние");
            Console.WriteLine("0 - Выход");
        }

        private static void ShowStateOnly()
        {
            string strategyName = GetEnemyStrategyName();
            Console.WriteLine($"Игрок: {_playerHp} HP | Противник: {_enemyHp} HP | Сила: {PlayerStrength} | Раунд: {_round} | Стратегия: {strategyName}");
        }

        private static bool ProcessPlayerTurn(int choice)
        {
            int damage = 0;
            string attackType = "";

            if (choice == 1)
            {
                damage = _lightStrategy.GetPlayerDamage(_round, _playerHp, _enemyHp);
                attackType = "лёгкий удар";
            }
            else if (choice == 2)
            {
                damage = _heavyStrategy.GetPlayerDamage(_round, _playerHp, _enemyHp);
                attackType = $"тяжёлый удар (база 12 + Сила×2 = {damage})";
            }
            else if (choice == 3)
            {
                _playerHp += 5;
                if (_playerHp > MaxHp)
                    _playerHp = MaxHp;
                Console.WriteLine("Игрок отдыхает и восстанавливает 5 HP.");
                return true;
            }

            if (choice == 1 || choice == 2)
            {
                int enemyHpBefore = _enemyHp;
                DamageContext context = new DamageContext(_round, enemyHpBefore, _playerHp, PlayerStrength);
                _pipeline.Apply(ref damage, context);

                _enemyHp -= damage;
                if (_enemyHp < 0) _enemyHp = 0;

                Console.WriteLine($"Игрок наносит {attackType}.");

                if (context.EnemyHpBeforeHit < 30 || context.PlayerHp < 40)
                {
                    Console.WriteLine($"  (После модификаторов урон составил {damage})");
                }
            }

            return true;
        }

        private static bool ProcessEnemyTurn()
        {
            int damage = _enemyStrategy.GetEnemyDamage(_round, _enemyHp, _playerHp);
            _playerHp -= damage;
            if (_playerHp < 0) _playerHp = 0;

            Console.WriteLine($"Противник наносит {damage} урона.");
            return true;
        }

        private static void UpdateEnemyStrategy()
        {
            if (!_hasSwitchedToRandom && _enemyHp < 25)
            {
                _enemyStrategy = new RandomEnemyStrategy();
                _hasSwitchedToRandom = true;
                _hasSwitchedToCareful = true; 
                Console.WriteLine("  (Противник меняет стратегию на Random!)");
            }
            else if (!_hasSwitchedToCareful && _enemyHp < 50)
            {
                _enemyStrategy = new CarefulEnemyStrategy();
                _hasSwitchedToCareful = true;
                Console.WriteLine("  (Противник меняет стратегию на Careful!)");
            }
        }

        private static string GetEnemyStrategyName()
        {
            if (_enemyStrategy is AggressiveEnemyStrategy)
                return "Aggressive";
            if (_enemyStrategy is CarefulEnemyStrategy)
                return "Careful";
            if (_enemyStrategy is RandomEnemyStrategy)
                return "Random";
            return "Unknown";
        }

        private static int ReadIntInput(int min, int max, string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                    return result;
                Console.WriteLine("Неверный ввод");
            }
        }
    }
}