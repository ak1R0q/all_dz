using System;

namespace first_dz
{
    public class PlayerProfile
    {
        public string PlayerName { get; }
        public int MaxLevel { get; set; }
        public int Score { get; set; }

        public PlayerProfile(string playerName, int maxLevel, int score)
        {
            PlayerName = playerName;
            MaxLevel = maxLevel;
            Score = score;
        }
    }
}