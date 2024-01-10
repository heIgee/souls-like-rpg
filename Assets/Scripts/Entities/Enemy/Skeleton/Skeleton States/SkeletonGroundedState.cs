public class SkeletonGroundedState : SkeletonState
{
    public SkeletonGroundedState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) 
        : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if ((skeleton.IsPlayerVisible || skeleton.IsPlayerNearby) && !player.Stats.IsDead)
            // why isn't it working? is there something missing?
            stateMachine.ChangeState(skeleton.BattleState);
    }
}
