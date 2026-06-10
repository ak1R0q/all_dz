namespace third_dz
{
    public class CarefulEnemyStrategy : IEnemyAttackStrategy
    {
        private const int Damage = 8;

        public int GetEnemyDamage(int roundIndex, int enemyHp, int playerHp)
        {
            return Damage;
        }
    }
}