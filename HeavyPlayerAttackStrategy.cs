namespace third_dz
{
    public class HeavyPlayerAttackStrategy : IPlayerAttackStrategy
    {
        private readonly int _baseDamage;
        private readonly int _strength;
        private readonly int _step;

        public HeavyPlayerAttackStrategy(int baseDamage, int strength, int step)
        {
            _baseDamage = baseDamage;
            _strength = strength;
            _step = step;
        }

        public int GetPlayerDamage(int roundIndex, int playerHp, int enemyHp)
        {
            return _baseDamage + _strength * _step;
        }
    }
}