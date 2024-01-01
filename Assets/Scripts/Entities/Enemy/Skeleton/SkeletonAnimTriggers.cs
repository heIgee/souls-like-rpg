using UnityEngine;

public class SkeletonAnimTriggers : MonoBehaviour
{
    private Skeleton Skeleton => GetComponentInParent<Skeleton>();
    
    private void AnimationTrigger() => Skeleton.AnimationTrigger();

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Skeleton.attackCheck.position, Skeleton.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                Skeleton.Stats.DoPhysicalDamage(target);
            }
    }

    private void OpenCounterWindow() => Skeleton.OpenCounterAttackWindow();
    private void CloseCounterWindow() => Skeleton.CloseCounterAttackWindow();

}
