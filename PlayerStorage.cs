using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace first_dz
{
    public static class PlayerStorage
    {
        private const string PlayersFile = "players.txt";

        public static List<PlayerProfile> LoadAllPlayers()
        {
            var players = new List<PlayerProfile>();

            if (!File.Exists(PlayersFile))
                return players;

            string[] lines = File.ReadAllLines(PlayersFile);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("PlayerName:") && i + 2 < lines.Length)
                {
                    string name = lines[i].Substring("PlayerName:".Length).Trim();
                    string maxLevelLine = lines[i + 1];
                    string scoreLine = lines[i + 2];

                    if (maxLevelLine.StartsWith("MaxLevel:") && scoreLine.StartsWith("Score:"))
                    {
                        int maxLevel = int.Parse(maxLevelLine.Substring("MaxLevel:".Length).Trim());
                        int score = int.Parse(scoreLine.Substring("Score:".Length).Trim());
                        players.Add(new PlayerProfile(name, maxLevel, score));
                    }

                    while (i + 1 < lines.Length && lines[i + 1] != "---")
                        i++;
                }
            }
            return players;
        }

        public static void SaveAllPlayers(List<PlayerProfile> players)
        {
            using (StreamWriter writer = new StreamWriter(PlayersFile))
            {
                for (int i = 0; i < players.Count; i++)
                {
                    writer.WriteLine($"PlayerName: {players[i].PlayerName}");
                    writer.WriteLine($"MaxLevel: {players[i].MaxLevel}");
                    writer.WriteLine($"Score: {players[i].Score}");
                    if (i != players.Count - 1)
                        writer.WriteLine("---");
                }
            }
        }
    }
}