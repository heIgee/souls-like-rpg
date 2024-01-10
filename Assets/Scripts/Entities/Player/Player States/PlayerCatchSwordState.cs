using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.Fx.PlayDustFX();
        player.Fx.ShakeScreen();

        sword = player.ThrownSword.transform;

        // turn towards coming sword
        if (player.transform.position.x > sword.position.x && player.FacingRight
         || player.transform.position.x < sword.position.x && !player.FacingRight)
            player.Flip();

        // not SetVelocity cause it's flipping the player
        rb.velocity = new Vector2(player.swordReturningForce * -player.FacingDirection, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(nameof(player.BusyFor), 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (trigerCalled)
            stateMachine.ChangeState(player.IdleState);
    }
}
