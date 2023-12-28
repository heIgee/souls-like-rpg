using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharStats
{
    private PlayerItemDrop dropSystem;

    protected override void Start()
    {
        base.Start();

        dropSystem = GetComponent<PlayerItemDrop>();
    }

    public override void DoPhysicalDamage(CharStats target, bool includeAmulet = false)
    {
        base.DoPhysicalDamage(target, includeAmulet);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        dropSystem.GenerateDrop();

        base.Die();
    }

    protected override void DecreaseHealth(int damage)
    {
        base.DecreaseHealth(damage);

        if (Inventory.instance.TryGetEquipment(EquipmentType.Armor, out var armor))
            armor.ExecuteEffects(holder.transform);
    }
}
