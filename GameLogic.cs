using System;
using System.Collections.Generic;

namespace first_dz
{
    public static class GameLogic
    {
        public static void PlayLevel(int level, PlayerProfile player, List<PlayerProfile> allPlayers)
        {
            int maxNumber = GetMaxNumberByLevel(level);
            Random random = new Random();
            int secretNumber = random.Next(1, maxNumber + 1);
            int guess;
            int attempts = 0;

            Console.WriteLine($"Я загадал число от 1 до {maxNumber}. угадай.");

            do
            {
                Console.Write("Твое число: ");
                while (!int.TryParse(Console.ReadLine(), out guess))
                {
                    Console.Write("Введи целое число: ");
                }

                attempts++;
                if (guess < secretNumber)
                    Console.WriteLine("Больше.");
                else if (guess > secretNumber)
                    Console.WriteLine("Меньше.");
                else
                    Console.WriteLine($"Поздравляю! ты угадал число {secretNumber} за {attempts} попыток!");

            } while (guess != secretNumber);

            int pointsEarned = level * level * 10;
            player.Score += pointsEarned;
            Console.WriteLine($"Ты получил {pointsEarned} очков!");

            if (level == player.MaxLevel && player.MaxLevel < 5)
            {
                player.MaxLevel++;
                Console.WriteLine($"Твой максимальный уровень повышен до {player.MaxLevel}!");
            }

            PlayerStorage.SaveAllPlayers(allPlayers);
        }

        private static int GetMaxNumberByLevel(int level)
        {
            switch (level)
            {
                case 1: return 10;
                case 2: return 50;
                case 3: return 100;
                case 4: return 250;
                case 5: return 1000;
                default: return 100;
            }
        }
    }
}