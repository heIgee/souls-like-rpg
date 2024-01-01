using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce = 10f;

    private float baseMoveSpeed;
    private float baseJumpForce;

    [Header("Dash info")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;

    private float baseDashSpeed;

    // TODO: gradual acceleration (lower distance, lower impact)
    public float swordReturningForce = 5f; 

    public float DashDirection { get; private set; }

    [Header("Attack details")]
    public Vector2[] AttackMovement;
    public float CounterAttackDuration = 0.2f;

    public bool IsBusy;

    public IEnumerator BusyFor(float seconds)
    {
        IsBusy = true;
        yield return new WaitForSeconds(seconds);
        IsBusy = false;
    }

    public void SetBusyFor(float seconds) => StartCoroutine(BusyFor(seconds));

    public SkillManager Skill { get; private set; }
    public GameObject ThrownSword { get; private set; } 

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
    public PlayerBlackHoleState BlackHoleState { get; private set; }

    public PlayerDeadState DeadState { get; private set; }

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
        BlackHoleState = new PlayerBlackHoleState(this, StateMachine, "Jump");

        DeadState = new PlayerDeadState(this, StateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log("Player started");

        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
        baseDashSpeed = dashSpeed;

        StateMachine.Initialize(IdleState);
        Skill = SkillManager.instance;
    }

    protected override void Update()
    {
        base.Update();

        // this throws nullreference when updating script while game is running (don't care)
        StateMachine?.CurrentState?.Update(); 

        // dash requires separate input check
        CheckDash();

        if (Input.GetKeyDown(KeyCode.F))
            Skill.Crystal.AttemptUse();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.AttemptUseFlask();
    }

    #region Sword
    public bool SwordAvailable
    {
        get
        {
            if (!ThrownSword) // sword with us
                return true;

            // no sword, return it
            ThrownSword.GetComponent<SwordController>().ReturnSword();
            return false;
        }
    }
    public void ThrowSword(GameObject newSword) => ThrownSword = newSword;
    public void CatchSword()
    {
        StateMachine.ChangeState(CatchSwordState);
        Destroy(ThrownSword);
    }
    #endregion

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public void CheckDash()
    {
        if (!IsWallDetected
         && !IsBusy
         && Input.GetKeyDown(KeyCode.LeftShift)
         && SkillManager.instance.Dash.AttemptUse())
        {
            DashDirection = Input.GetAxisRaw("Horizontal");

            if (DashDirection == 0)
                DashDirection = FacingDirection;

            StateMachine.ChangeState(DashState);
        }
    }

    public override void SlowBy(float slowPercentage, float slowDuration)
    {
        slowPercentage = Mathf.Clamp01(slowPercentage);

        moveSpeed *= 1 - slowPercentage;
        jumpForce *= 1 - slowPercentage;
        dashSpeed *= 1 - slowPercentage;
        Anim.speed *= 1 - slowPercentage;

        Invoke(nameof(RestoreBaseSpeed), slowDuration);
    }
    protected override void RestoreBaseSpeed()
    {
        base.RestoreBaseSpeed();

        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
        dashSpeed = baseDashSpeed;
    }

    public override void Die()
    {
        StateMachine.ChangeState(DeadState);
    }
}
