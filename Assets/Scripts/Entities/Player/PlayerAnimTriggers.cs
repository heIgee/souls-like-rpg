using UnityEngine;

public class PlayerAnimTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger() => player.AnimationTrigger();
    
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats target = hit.GetComponent<EnemyStats>();

                player.Stats.DoPhysicalDamage(target);

                // TODO: this should be unlocked idk
                player.Stats.DoMagicalDamage(target);

                if (Inventory.instance.TryGetEquipment(EquipmentType.Weapon, out var equippedItem))
                    equippedItem.ExecuteEffects(target.transform);
            }
    }

    private void ThrowSword()
    {
        SkillManager.instance.Sword.ThrowSword();
    }
}
