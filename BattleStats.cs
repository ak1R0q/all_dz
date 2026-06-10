using System;

namespace chetv_dz
{
    public class BattleStats
    {
        public int PlayerHP { get; set; }
        public int EnemyHP { get; set; }
        public int Tick { get; set; }
        public DateTime StartTime { get; set; }

        public BattleStats()
        {
            PlayerHP = 100;
            EnemyHP = 300;
            Tick = 0;
            StartTime = DateTime.UtcNow;
        }

        public BattleStats(int playerHP, int enemyHP, int tick, DateTime startTime)
        {
            PlayerHP = playerHP;
            EnemyHP = enemyHP;
            Tick = tick;
            StartTime = startTime;
        }
    }
}