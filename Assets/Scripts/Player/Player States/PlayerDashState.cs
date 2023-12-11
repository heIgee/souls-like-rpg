using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.dashDuration;

        player.Skill.Clone.CreateClone(player.transform);
    }

    public override void Exit()
    {
        base.Exit();

        // why
        player.SetVelocity(xInput * 0.8f * player.moveSpeed, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();


        if (!player.IsGroundDetected && player.IsWallDetected)
            stateMachine.ChangeState(player.WallSlideState);

        // no vertical movement while dashing
        player.SetVelocity(player.dashSpeed * player.DashDirection, 0);

        if(stateTimer < 0) 
            stateMachine.ChangeState(player.IdleState);
    }
}
