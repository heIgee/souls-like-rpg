using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private readonly float flyTime = 0.4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackHoleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillUsed = false;
        stateTimer = flyTime;

        player.IsBusy = true;

        defaultGravity = rb.gravityScale;
        rb.gravityScale = 0f;
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = defaultGravity;
        player.Fx.SetTransparency(false);

        player.IsBusy = false;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 15);

        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -0.1f);

            if (!skillUsed)
            {
                if (player.Skill.BlackHole.AttemptUse())
                    skillUsed = true;
            }
        }

        if (player.Skill.BlackHole.SkillFinished())
            stateMachine.ChangeState(player.AirState);
    }
}
