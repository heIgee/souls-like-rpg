using UnityEngine;

public class SkeletonIdleState : SkeletonGroundedState
{
    public SkeletonIdleState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) 
        : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skeleton.SetVelocity(0, rb.velocity.y);

        // must be integer for holistic animation
        stateTimer = 2f + Random.Range(-1, 1);
    }

    public override void Exit()
    {
        base.Exit();

        if (!skeleton.IsGroundDetected || skeleton.IsWallDetected)
            skeleton.Flip();

        AudioManager.instance.PlaySFX(24, enemy.transform); // bones sound
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
            stateMachine.ChangeState(skeleton.MoveState);
    }
}
