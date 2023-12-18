using UnityEngine;

public class SkeletonAnimationTriggers : MonoBehaviour
{
    private Skeleton skeleton => GetComponentInParent<Skeleton>();
    
    private void AnimationTrigger() => skeleton.AnimationTrigger();

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skeleton.attackCheck.position, skeleton.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                skeleton.Stats.DoDamage(target);
            }
    }

    private void OpenCounterWindow() => skeleton.OpenCounterAttackWindow();
    private void CloseCounterWindow() => skeleton.CloseCounterAttackWindow();

}
