using UnityEngine;

public class PlayerStats : CharStats
{
    private Player player; 
    private PlayerItemDrop dropSystem;

    protected override void Start()
    {
        base.Start();

        player = holder as Player;
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

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;
    }

    protected override void DecreaseHealth(int damage)
    {
        base.DecreaseHealth(damage);

        if (Inventory.instance.TryGetEquipment(EquipmentType.Armor, out var armor))
            armor.ExecuteEffects(player.transform);
    }

    public override void OnEvade(Transform enemyTransform)
    {
        player.Skill.Dodge.CreateMirageOnDodge(enemyTransform);
    }

    public void DoCloneDamage(CharStats target, float damageMultiplier, bool includeAmulet = true)
    {
        if (includeAmulet && Inventory.instance.TryGetEquipment(EquipmentType.Amulet, out var amulet))
        {
            //Debug.LogWarning(amulet);
            amulet.ExecuteEffects(target.transform);
        }

        if (target.AttemptAvoid(this))
            return;

        int totalPhysDamage = damage.Value;

        if (damageMultiplier > 0) 
            totalPhysDamage = Mathf.RoundToInt(totalPhysDamage * damageMultiplier);

        if (AttemptCrit())
        {
            totalPhysDamage = CalculateCrit(totalPhysDamage);
            Debug.LogWarning($"{gameObject.name} clone performed CRIT [{totalPhysDamage}]" +
                $" on {target.gameObject.name}");
        }

        if (target.isChilled)
            totalPhysDamage -= Mathf.RoundToInt(target.armor.Value * chilledArmorDebuff);
        else
            totalPhysDamage -= target.armor.Value;

        if (totalPhysDamage < 0)
            totalPhysDamage = 0;

        target.TakeDamage(totalPhysDamage);
    }
}
