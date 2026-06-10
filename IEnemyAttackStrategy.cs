namespace third_dz
{
    public interface IEnemyAttackStrategy
    {
        int GetEnemyDamage(int roundIndex, int enemyHp, int playerHp);
    }
}