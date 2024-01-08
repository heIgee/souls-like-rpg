using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Item Data/Item effect/Heal effect")]
public class HealEffect : ItemEffect
{
    [Range(0f, 100f)]
    [SerializeField] private float healPercent;
    public override void Execute(Transform spawnTransform = null)
    {
        PlayerStats stats = PlayerManager.instance.player.Stats as PlayerStats;

        int healAmount = Mathf.RoundToInt(stats.maxHp.Value * healPercent / 100f);

        stats.IncreaseHealth(healAmount);
    }
}