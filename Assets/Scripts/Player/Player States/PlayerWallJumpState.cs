using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(-5 * player.FacingDirection, player.jumpForce);

        stateTimer = 0.4f;
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

        if (stateTimer < 0) 
            stateMachine.ChangeState(player.AirState);

        if(player.IsGroundDetected)
            stateMachine.ChangeState(player.IdleState);
    }

}
