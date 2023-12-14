using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SkeletonBattleState : SkeletonState
{
    public SkeletonBattleState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = sk.battleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();





        if (player.transform.position.x < sk.transform.position.x && sk.FacingRight
         || player.transform.position.x > sk.transform.position.x && !sk.FacingRight)
            // turn towards player
            sk.Flip();

        if (sk.IsPlayerVisible || sk.IsPlayerNearby) // is ready to approach player
        {
            stateTimer = sk.battleTime;

            // run towards player like a stupid mf
            sk.SetVelocity(sk.moveSpeed * 1.4f * sk.FacingDirection, rb.velocity.y);

            // RaycastHit2D.distance returns 0 if no collision, keep in mind
            if (sk.IsPlayerVisible && sk.IsPlayerVisible.distance < sk.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(sk.AttackState);
                else
                    sk.SetZeroVelocity();

                animator.SetBool(animBoolName, false);
            }
            else
                animator.SetBool(animBoolName, true);


        }
        else if (stateTimer < 0 || Vector2.Distance(sk.transform.position, player.transform.position) > 10)
            // player is too far or battle time expired
            stateMachine.ChangeState(sk.IdleState);

    }

    private bool CanAttack()
    {
        if (Time.time >= sk.lastAttackTime + sk.attackCooldown)
        {
            sk.lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
}
