 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce = 10f;

    [Header("Dash info")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float DashDirection { get; private set; }

    [Header("Attack details")]
    public Vector2[] AttackMovement;
    public float CounterAttackDuration = 0.2f;

    public bool IsBusy { get; private set; }

    public IEnumerator BusyFor(float seconds)
    {
        IsBusy = true;
        yield return new WaitForSeconds(seconds);
        IsBusy = false;
    }

    public SkillManager Skill { get; private set; }

    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    public PlayerPrimaryAttackState PrimaryAttackState { get; private set; }
    public PlayerCounterAttackState CounterAttackState { get; private set; }
    public PlayerAimSwordState AimSwordState { get; private set; }
    public PlayerCatchSwordState CatchSwordState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState  = new PlayerAirState(this, StateMachine, "Jump");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "WallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, "Jump");
        DashState = new PlayerDashState(this, StateMachine, "Dash");

        PrimaryAttackState = new PlayerPrimaryAttackState(this, StateMachine, "Attack");
        CounterAttackState = new PlayerCounterAttackState(this, StateMachine, "CounterAttack");
        AimSwordState = new PlayerAimSwordState(this, StateMachine, "AimSword");
        CatchSwordState = new PlayerCatchSwordState(this, StateMachine, "CatchSword");
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log("Player started");

        if (Animator != null)
        {
            Debug.Log("Animator found and assigned successfully.");
        }
        else
        {
            Debug.LogError("Animator not found. Check the hierarchy and component setup.");
        }

        StateMachine.Initialize(IdleState);
        Skill = SkillManager.instance;
    }

    protected override void Update()
    {
        base.Update();

        StateMachine?.CurrentState?.Update(); // this throws nullreference when updating script while game is running
        CheckDash();
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public void CheckDash()
    {
        if (!IsWallDetected
            && Input.GetKeyDown(KeyCode.LeftShift)
            && SkillManager.instance.Dash.CanUse())
        {
            DashDirection = Input.GetAxisRaw("Horizontal");

            if (DashDirection == 0)
                DashDirection = FacingDirection;

            StateMachine.ChangeState(DashState);
        }
    }
}
