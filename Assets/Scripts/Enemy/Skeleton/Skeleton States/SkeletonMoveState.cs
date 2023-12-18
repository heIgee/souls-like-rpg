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

        if(stateTimer < 0 || !sk.IsGroundDetected)
            stateMachine.ChangeState(sk.IdleState);

        sk.SetVelocity(sk.moveSpeed * sk.FacingDirection, rb.velocity.y);
    }
}
