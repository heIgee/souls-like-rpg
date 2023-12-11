using System.Collections;
using System.Collections.Generic;
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
                hit.GetComponent<Player>().Damage();
    }

    private void OpenCounterWindow() => skeleton.OpenCounterAttackWindow();
    private void CloseCounterWindow() => skeleton.CloseCounterAttackWindow();

}
