using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharStats
{
    protected override void Start()
    {
        base.Start();
    }

    public override void DoDamage(CharStats target)
    {
        base.DoDamage(target);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }


}
