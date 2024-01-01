using UnityEngine;

public class SkeletonMoveState : SkeletonGroundedState
{
    public SkeletonMoveState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 5f + Random.Range(-1, 1);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0 || !skeleton.IsGroundDetected)
            stateMachine.ChangeState(skeleton.IdleState);

        skeleton.SetVelocity(skeleton.moveSpeed * skeleton.FacingDirection, rb.velocity.y);
    }
}
