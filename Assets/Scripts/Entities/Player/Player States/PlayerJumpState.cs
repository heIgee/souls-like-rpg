using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(xInput != 0)
            player.SetVelocity(xInput * 0.8f * player.moveSpeed, rb.velocity.y);

        if (player.IsWallDetected)
            stateMachine.ChangeState(player.WallSlideState);

        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.AirState);
    }
}
