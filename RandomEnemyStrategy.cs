using System;

namespace third_dz
{
    public class RandomEnemyStrategy : IEnemyAttackStrategy
    {
        private readonly Random _random = new Random(12345);
        private readonly int[] _damages = { 10, 12, 14 };

        public int GetEnemyDamage(int roundIndex, int enemyHp, int playerHp)
        {
            int index = _random.Next(0, 3);
            return _damages[index];
        }
    }
}