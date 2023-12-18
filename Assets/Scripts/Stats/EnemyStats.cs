using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharStats
{
    private Enemy enemy;
    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    public override void DoDamage(CharStats target)
    {
        base.DoDamage(target);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        enemy.DamageFX();
    }

    protected override void Die()
    {
        enemy.Die();
    }


}
