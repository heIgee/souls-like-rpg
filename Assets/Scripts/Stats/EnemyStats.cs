using UnityEngine;

public class EnemyStats : CharStats
{
    private ItemDrop dropSystem;
    public Stat soulsDrop;

    [Header("Level info")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.4f;

    protected override void Start()
    {
        // must be above base.Start() to ensure base values like health = maxhp
        soulsDrop.SetBaseValue(2);
        AdjustStatsToLevel();

        base.Start();

        dropSystem = GetComponent<ItemDrop>();
    }

    private void AdjustStatsToLevel()
    {
        AdjustStat(damage);
        AdjustStat(maxHp);
        AdjustStat(armor);

        AdjustStat(soulsDrop);

        // should I adjust some other stats?
    }

    private void AdjustStat(Stat stat)
    {
        int statValue = 0;

        // start increasing from 2nd level
        for (int i = 1; i < level; i++)
        {
            float levelModifier = stat.Value * percentageModifier;
            statValue += Mathf.RoundToInt(levelModifier);
        }

        stat.AddModifier(statValue);
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

        // TODO: some pretty fx
        PlayerManager.instance.currency += soulsDrop.Value;
    }
}
