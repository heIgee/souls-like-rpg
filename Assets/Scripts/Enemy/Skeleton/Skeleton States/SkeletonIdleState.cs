using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sk.SetVelocity(0, rb.velocity.y);
        stateTimer = 2f;
    }

    public override void Exit()
    {
        base.Exit();

        if (!sk.IsGroundDetected || sk.IsWallDetected)
            sk.Flip();
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
            stateMachine.ChangeState(sk.MoveState);
    }
}
