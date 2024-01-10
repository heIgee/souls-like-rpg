using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        if (Input.GetKeyDown(KeyCode.R) && player.Skill.BlackHole.blackHoleUnlocked)
        {
            // black hole is used differently, so popup in AttemptUse wont be called
            if (player.Skill.BlackHole.cooldownTimer < 0)
                stateMachine.ChangeState(player.BlackHoleState);
            else
                player.Fx.CreatePopupText("Ultimate on cooldown");
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && player.SwordAvailable && player.Skill.Sword.swordUnlocked)
            stateMachine.ChangeState(player.AimSwordState);

        if (Input.GetKeyDown(KeyCode.Q) && player.Skill.Parry.parryUnlocked) // TODO: counter-attack cooldown
            stateMachine.ChangeState(player.CounterAttackState);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.PrimaryAttackState);
        
        if (!player.IsGroundDetected)
            stateMachine.ChangeState(player.AirState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected)
            stateMachine.ChangeState(player.JumpState);
    }
}