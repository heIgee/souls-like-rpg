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

        skeleton.lastAttackTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        skeleton.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(skeleton.BattleState);
    }
}
