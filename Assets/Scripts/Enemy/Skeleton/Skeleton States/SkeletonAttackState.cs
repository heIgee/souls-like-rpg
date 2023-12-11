using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : SkeletonState
{
    public SkeletonAttackState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        sk.lastAttackTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        sk.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(sk.BattleState);
    }
}
