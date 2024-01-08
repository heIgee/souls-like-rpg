using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    public List<int> modifiers = new();

    public int Value => baseValue + modifiers.Sum();

    public virtual void SetBaseValue(int value) => baseValue = value;
    public virtual void AddModifier(int modifier)
    {
        // zero modifiers are pretty stupid, aren't they?
        if (modifier != 0)
            modifiers.Add(modifier);
    }

    public virtual void RemoveModifier(int modifier) => modifiers.Remove(modifier);

    public static Stat GetStatReference(CharStats stats, StatType statType) 
    {
        if (stats == null)
        {
            Debug.LogError($"Passed {nameof(CharStats)} is null");
            return null;
        }

        return statType switch
        {
            StatType.Strength => stats.strength,
            StatType.Agility => stats.agility,
            StatType.Intelligence => stats.intelligence,
            StatType.Vitality => stats.vitality,

            StatType.Health => stats.maxHp,
            StatType.Armor => stats.armor,
            StatType.Evasion => stats.evasion,
            StatType.MagicRes => stats.magicRes,

            StatType.Damage => stats.damage,
            StatType.CritChance => stats.critChance,
            StatType.CritDamage => stats.critDamage,

            StatType.FireDamage => stats.fireDamage,
            StatType.IceDamage => stats.iceDamage,
            StatType.LightningDamage => stats.lightningDamage,

            // add cases for other StatType values
            _ => throw new ArgumentOutOfRangeException(nameof(statType), statType, null),
        };
    }

    //public static explicit operator Stat(StatType statType)
    //{
        
    //}
}
