namespace third_dz
{
    public class DamageContext
    {
        public int RoundIndex { get; }
        public int EnemyHpBeforeHit { get; }
        public int PlayerHp { get; }
        public int Strength { get; }

        public DamageContext(int roundIndex, int enemyHpBeforeHit, int playerHp, int strength)
        {
            RoundIndex = roundIndex;
            EnemyHpBeforeHit = enemyHpBeforeHit;
            PlayerHp = playerHp;
            Strength = strength;
        }
    }
}