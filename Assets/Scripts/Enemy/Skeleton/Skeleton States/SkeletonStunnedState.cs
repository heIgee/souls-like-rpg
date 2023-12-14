using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : SkeletonState
{
    public SkeletonStunnedState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = sk.stunDuration;

        if(sk.IsGroundDetected)
            sk.SetVelocity(sk.stunDirection.x  * -sk.FacingDirection, sk.stunDirection.y);

        sk.Fx.StartRedWhiteBlink();
    }

    public override void Exit()
    {
        base.Exit();

        sk.Fx.CancelRedWhiteBlink();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(sk.BattleState);
    }
}
