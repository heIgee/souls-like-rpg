using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Item Data/Item effect/Buff effect")]
public class BuffEffect : ItemEffect
{
    private PlayerStats stats;

    [SerializeField] private StatType statType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void Execute(Transform spawnTransform = null)
    {
        stats = PlayerManager.instance.player.Stats as PlayerStats;
        var statRef = Stat.GetStatReference(stats, statType);

        if (statRef != null)
            stats.BuffStat(statRef, buffAmount, buffDuration);
        else
            Debug.LogError("Stat reference is null");
    }
}
