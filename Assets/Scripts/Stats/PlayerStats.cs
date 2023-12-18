using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharStats
{
    private Player player;
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void DoDamage(CharStats target)
    {
        base.DoDamage(target);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        player.DamageFX();
    }

    protected override void Die()
    {
        player.Die();
    }
}
