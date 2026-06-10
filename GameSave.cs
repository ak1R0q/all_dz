using System;

namespace chetv_dz
{
    [Serializable]
    public class GameSave
    {
        public int PlayerHP { get; set; }
        public int EnemyHP { get; set; }
        public int Tick { get; set; }
        public DateTime StartTime { get; set; }

        public GameSave() { }

        public GameSave(BattleStats stats)
        {
            PlayerHP = stats.PlayerHP;
            EnemyHP = stats.EnemyHP;
            Tick = stats.Tick;
            StartTime = stats.StartTime;
        }

        public void ApplyTo(BattleStats stats)
        {
            stats.PlayerHP = PlayerHP;
            stats.EnemyHP = EnemyHP;
            stats.Tick = Tick;
            stats.StartTime = StartTime;
        }
    }
}