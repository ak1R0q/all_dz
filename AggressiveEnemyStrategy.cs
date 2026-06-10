namespace third_dz
{
    public class AggressiveEnemyStrategy : IEnemyAttackStrategy
    {
        private const int Damage = 15;

        public int GetEnemyDamage(int roundIndex, int enemyHp, int playerHp)
        {
            return Damage;
        }
    }
}