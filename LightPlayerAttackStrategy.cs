namespace third_dz
{
    public class LightPlayerAttackStrategy : IPlayerAttackStrategy
    {
        private const int Damage = 10;

        public int GetPlayerDamage(int roundIndex, int playerHp, int enemyHp)
        {
            return Damage;
        }
    }
}