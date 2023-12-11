using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.CounterAttackDuration;
        animator.SetBool("CounterAttackSuccessful", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10f; // just value bigger than 1 to not exit the state
                    animator.SetBool("CounterAttackSuccessful", true);
                }

        if (stateTimer < 0 || trigerCalled)
            stateMachine.ChangeState(player.IdleState);
    }
}
