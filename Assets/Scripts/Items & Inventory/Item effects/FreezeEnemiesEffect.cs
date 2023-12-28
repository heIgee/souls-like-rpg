using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Item Data/Item effect/Freeze enemies effect")]
public class FreezeEnemiesEffect : ItemEffect
{
    [SerializeField] private float duration;
    public override void Execute(Transform spawnTransform = null)
    {
        PlayerStats stats = PlayerManager.instance.player.Stats as PlayerStats;

        Debug.LogWarning($"hp: {stats.CurrentHp}, max : {stats.maxHp.Value}");
        if (stats.CurrentHp >= stats.maxHp.Value * 0.2f)
            return;

        if (!Inventory.instance.AttemptUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnTransform.position, 5f);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().FreezeTimeFor(duration);
    }
}
