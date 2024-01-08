using System;
using System.Collections.Generic;

// came up with it for convenient stat updates
[Serializable]
public class MajorStat : Stat
{
    public readonly List<(Stat affectedStat, int buffMultiplier)> effectRecords = new();

    public void AddAffectedStat(Stat affectedStat, int buffAmount)
    {
        if (buffAmount != 0)
            effectRecords.Add((affectedStat, buffAmount));
    }

    public override void SetBaseValue(int value)
    {
        base.SetBaseValue(value);
        ApplyEffects();
    }

    public override void AddModifier(int modifier)
    {
        RemoveEffects();
        base.AddModifier(modifier);
        ApplyEffects();
    }

    public override void RemoveModifier(int modifier)
    {
        RemoveEffects();
        base.RemoveModifier(modifier);
        ApplyEffects();
    }

    private void ApplyEffects()
    {
        if (Value <= 0)
            return;

        foreach (var (affectedStat, buffMultiplier) in effectRecords)
            affectedStat.AddModifier(Value * buffMultiplier);
    }

    public void RemoveEffects()
    {
        foreach (var (affectedStat, buffMultiplier) in effectRecords)
            affectedStat.RemoveModifier(Value * buffMultiplier);
    }
}
