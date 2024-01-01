public class SkeletonStunnedState : SkeletonState
{
    public SkeletonStunnedState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = skeleton.stunDuration;

        if(skeleton.IsGroundDetected)
            skeleton.SetVelocity(skeleton.stunDirection.x  * -skeleton.FacingDirection, skeleton.stunDirection.y);

        skeleton.Fx.StartRedWhiteBlink();
    }

    public override void Exit()
    {
        base.Exit();

        skeleton.Fx.CancelFX();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(skeleton.BattleState);
    }
}
