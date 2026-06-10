namespace third_dz
{
    public interface IPlayerAttackStrategy
    {
        int GetPlayerDamage(int roundIndex, int playerHp, int enemyHp);
    }
}