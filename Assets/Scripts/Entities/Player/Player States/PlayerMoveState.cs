public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(14); // footsteps
        // TODO: has finite length
        // if starting in update, plays after exiting the state because of the way 
        // state update are being called
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(14); // footsteps
    }

    public override void Update()
    {
        base.Update();


        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (xInput == 0)
            stateMachine.ChangeState(player.IdleState);

    }
}

