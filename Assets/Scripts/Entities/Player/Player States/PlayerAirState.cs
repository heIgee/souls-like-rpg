using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        if (xInput != 0)
            player.SetVelocity(xInput * 0.8f * player.moveSpeed, rb.velocity.y);

        // I just want it
        if (Input.GetKeyDown(KeyCode.Q))
            stateMachine.ChangeState(player.CounterAttackState);

        if (player.IsGroundDetected)
            stateMachine.ChangeState(player.IdleState);

        if (player.IsWallDetected)
            stateMachine.ChangeState(player.WallSlideState);
    } 
}
