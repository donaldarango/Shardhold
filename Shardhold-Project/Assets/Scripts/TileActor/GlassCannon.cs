using UnityEngine;

public class GlassCannon : EnemyUnit
{
    public override void AttackBase()
    {
        base.AttackBase();

        TakeDamage(1);
    }

    public override void Attack(TileActor target)
    {
        base.Attack(target);

        TakeDamage(1);
    }
}
