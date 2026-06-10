using System;
using System.Collections.Generic;
using System.Linq;

namespace first_dz
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PlayerProfile> players = PlayerStorage.LoadAllPlayers();

            Console.Write("Введите имя игрока: ");
            string playerName = Console.ReadLine()?.Trim();

            PlayerProfile currentPlayer = players.FirstOrDefault(p => p.PlayerName == playerName);

            if (currentPlayer == null)
            {
                currentPlayer = new PlayerProfile(playerName, 1, 0);
                players.Add(currentPlayer);
                PlayerStorage.SaveAllPlayers(players);
                Console.WriteLine($"Создан новый игрок: {playerName}");
            }
            else
            {
                Console.WriteLine($"Добро пожаловать обратно, {playerName}!");
            }

            bool exit = false;

            while (!exit)
            {
                Console.WriteLine($"\nДоступные уровни: 1..{currentPlayer.MaxLevel}");
                int selectedLevel = 0;

                while (true)
                {
                    Console.Write($"Введите уровень (1..{currentPlayer.MaxLevel}): ");
                    if (int.TryParse(Console.ReadLine(), out selectedLevel) &&
                        selectedLevel >= 1 && selectedLevel <= currentPlayer.MaxLevel)
                    {
                        break;
                    }
                    Console.WriteLine("Ошибка ввода. попробуйте снова.");
                }

                GameLogic.PlayLevel(selectedLevel, currentPlayer, players);

                Console.WriteLine($"Текущие данные: Игрок {currentPlayer.PlayerName}, Макс. уровень {currentPlayer.MaxLevel}, Счёт {currentPlayer.Score}.");

                Console.WriteLine("1 - Выбрать уровень, 2 - Выйти");
                string choice = Console.ReadLine();
                if (choice == "2")
                {
                    exit = true;
                }
                else if (choice != "1")
                {
                    Console.WriteLine("Неверный выбор. Завершаем программу.");
                    exit = true;
                }
            }

            Console.WriteLine("\nУровень сложности задания: 3");
            Leaderboard.SaveLeaderboard(players);
            Leaderboard.ShowLeaderboard();
        }
    }
}