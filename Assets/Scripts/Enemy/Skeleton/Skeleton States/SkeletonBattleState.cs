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

        //Debug.Log("IsPlayerVisible.distance: " + sk.IsPlayerVisible.distance); // often returns 0 ??
        // RaycastHit2D.distance returns 0 if no collision!
        //Debug.Log("attackDistance: "+ sk.attackDistance);

        //if (sk.IsPlayerVisible &&
        //   sk.IsPlayerVisible.distance > sk.attackDistance) 
        //    // prevent over approaching
        //    // TODO: It breaks long distance behaviour
            
        //    sk.SetVelocity(sk.moveSpeed * 1.4f * sk.FacingDirection, rb.velocity.y);

        if (sk.IsPlayerVisible || sk.IsPlayerNearby) // is ready to approach player
        {
            stateTimer = sk.battleTime;

            sk.SetVelocity(sk.moveSpeed * 1.4f * sk.FacingDirection, rb.velocity.y);

            if (CanAttack())
            {
                if (sk.IsPlayerVisible && sk.IsPlayerVisible.distance < sk.attackDistance)
                    stateMachine.ChangeState(sk.AttackState);
            }
        }
        else if (stateTimer < 0 || Vector2.Distance(sk.transform.position, player.transform.position) > 10)
            // player is too far or battle time expired
            stateMachine.ChangeState(sk.IdleState);


        if (player.transform.position.x < sk.transform.position.x && sk.FacingRight
        || player.transform.position.x > sk.transform.position.x && !sk.FacingRight)
            // turn to player
            sk.Flip();
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
