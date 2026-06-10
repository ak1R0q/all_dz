using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace first_dz
{
    public static class Leaderboard
    {
        private const string LeaderboardFile = "leaderboard.txt";

        public static void SaveLeaderboard(List<PlayerProfile> players)
        {
            using (StreamWriter writer = new StreamWriter(LeaderboardFile))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    writer.WriteLine($"PlayerName: {players[i].PlayerName}");
                    writer.WriteLine($"Level: {players[i].MaxLevel}");
                    writer.WriteLine($"Score: {players[i].Score}");
                    if (i != players.Count - 1)
                        writer.WriteLine("---");
                }
            }
        }

        public static void ShowLeaderboard()
        {
            if (!File.Exists(LeaderboardFile))
            {
                Console.WriteLine("Таблица лидеров пока пуста.");
                return;
            }

            var leaders = new List<(string Name, int Level, int Score)>();

            string[] lines = File.ReadAllLines(LeaderboardFile);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("PlayerName:") && i + 2 < lines.Length)
                {
                    string name = lines[i].Substring("PlayerName:".Length).Trim();
                    string levelLine = lines[i + 1];
                    string scoreLine = lines[i + 2];

                    if (levelLine.StartsWith("Level:") && scoreLine.StartsWith("Score:"))
                    {
                        int level = int.Parse(levelLine.Substring("Level:".Length).Trim());
                        int score = int.Parse(scoreLine.Substring("Score:".Length).Trim());
                        leaders.Add((name, level, score));
                    }
                    while (i + 1 < lines.Length && lines[i + 1] != "---")
                        i++;
                }
            }

            leaders = leaders.OrderByDescending(l => l.Score).ToList();

            Console.WriteLine("\n=== ТАБЛИЦА ЛИДЕРОВ ===");
            Console.WriteLine("| Игрок                | Уровень | Счёт     |");
            Console.WriteLine("|----------------------|---------|----------|");
            foreach (var leader in leaders)
            {
                Console.WriteLine($"| {leader.Name,-20} | {leader.Level,-7} | {leader.Score,-8} |");
            }
            Console.WriteLine("========================\n");
        }
    }
}