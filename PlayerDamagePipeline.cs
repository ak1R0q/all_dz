namespace third_dz
{
    public delegate void DamageModifier(ref int damage, DamageContext context);

    public class PlayerDamagePipeline
    {
        public event DamageModifier Modifiers;

        public void Apply(ref int damage, DamageContext context)
        {
            if (Modifiers != null)
            {
                foreach (DamageModifier modifier in Modifiers.GetInvocationList())
                {
                    modifier(ref damage, context);
                }
            }
        }

        public static void ApplyBonusIfEnemyLow(ref int damage, DamageContext context)
        {
            if (context.EnemyHpBeforeHit < 30)
            {
                damage += 5;
            }
        }

        public static void ApplyPenaltyIfPlayerLow(ref int damage, DamageContext context)
        {
            if (context.PlayerHp < 40)
            {
                damage -= 3;
                if (damage < 1)
                    damage = 1;
            }
        }
    }
}