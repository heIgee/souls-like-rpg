using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New equipment data", menuName = "Item Data/Equipment")]
public class EquipmentData : ItemData
{
    public EquipmentType equipmentType;
    public ItemEffect[] itemEffects;

    public float itemCooldown;
    public float lastTimeUsed;

    private void OnEnable()
    {
        // idk the better way to reset cooldown on start without Update()
        lastTimeUsed = -itemCooldown;
    }

    PlayerStats Stats => PlayerManager.instance.player.Stats as PlayerStats;

    [Header("Major stats")]
    public int strength; // +1 dmg and +1 crit dmg
    public int agility; // +1 evasion and +1 crit chance
    public int intelligence; // +1 magic dmg and +3 magic resistance
    public int vitality; // +3 health

    [Header("Defensive stats")]
    public int maxHp;
    public int armor;
    public int evasion;
    public int magicRes;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critDamage;  // default 150%

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    public void ExecuteEffects(Transform enemy = null)
    {
        foreach (var effect in itemEffects)
        {
            effect.Execute(enemy);
        }
    }

    // I have tangible questions about performance of it
    // but it will be absolute hell to implement it efficiently (I tried)
    public void AddModifiers()
    {
        //Debug.Log($"PM: {PlayerManager.instance}");
        //Debug.Log($"Payer: {PlayerManager.instance.player}");
        //Debug.Log($"Stats: {PlayerManager.instance.player.Stats}");

        AddIfNotZero(Stats.strength, strength);
        AddIfNotZero(Stats.agility, agility);
        AddIfNotZero(Stats.intelligence, intelligence);
        AddIfNotZero(Stats.vitality, vitality);

        AddIfNotZero(Stats.maxHp, maxHp);
        AddIfNotZero(Stats.armor, armor);
        AddIfNotZero(Stats.evasion, evasion);
        AddIfNotZero(Stats.magicRes, magicRes);

        AddIfNotZero(Stats.damage, damage);
        AddIfNotZero(Stats.critChance, critChance);
        AddIfNotZero(Stats.critDamage, critDamage);

        AddIfNotZero(Stats.fireDamage, fireDamage);
        AddIfNotZero(Stats.iceDamage, iceDamage);
        AddIfNotZero(Stats.lightningDamage, lightningDamage);
    }

    public void RemoveModifiers()
    {
        RemoveIfNotZero(Stats.strength, strength);
        RemoveIfNotZero(Stats.agility, agility);
        RemoveIfNotZero(Stats.intelligence, intelligence);
        RemoveIfNotZero(Stats.vitality, vitality);

        RemoveIfNotZero(Stats.maxHp, maxHp);
        RemoveIfNotZero(Stats.armor, armor);
        RemoveIfNotZero(Stats.evasion, evasion);
        RemoveIfNotZero(Stats.magicRes, magicRes);

        RemoveIfNotZero(Stats.damage, damage);
        RemoveIfNotZero(Stats.critChance, critChance);
        RemoveIfNotZero(Stats.critDamage, critDamage);

        RemoveIfNotZero(Stats.fireDamage, fireDamage);
        RemoveIfNotZero(Stats.iceDamage, iceDamage);
        RemoveIfNotZero(Stats.lightningDamage, lightningDamage);
    }

    private void AddIfNotZero(Stat stat, int value)
    {
        if (value != 0)
            stat.AddModifier(value);
    }

    private void RemoveIfNotZero(Stat stat, int value)
    {
        if (value != 0)
            stat.RemoveModifier(value);
    }



    public override string GetDescription()
    {
        sb.Clear();

        // I hate it, like everything else in this class
        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        AddItemDescription(maxHp, "Max health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(magicRes, "Magic Resistance");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Critical Chance");
        AddItemDescription(critDamage, "Critical Damage");

        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightningDamage, "Lightning Damage");

        AddEffectsDescription();

        return sb.ToString();
    }

    private void AddItemDescription(int value, string name)
    {
        if (value == 0)
            return;

        if (sb.Length > 0)
            sb.AppendLine();

        if (value > 0)
            sb.Append($"+ {value} {name}");
        else
            sb.Append($"- {value *= -1} {name}");
    }

    private void AddEffectsDescription()
    {
        if (itemEffects.Length <= 0)
            return;

        if (sb.Length > 0)
            sb.AppendLine();    

        foreach (var effect in itemEffects)
            if (!string.IsNullOrEmpty(effect.description))
                sb.AppendLine(effect.description);
    }
}
