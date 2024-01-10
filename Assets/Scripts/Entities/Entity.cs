using System;
using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("Knockback info")]
    public Vector2 knockbackVector;
    // what the fuck is knockback duration
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    public int KnockbackDir { get; protected set; } = 1;
    public int FacingDirection { get; protected set; } = 1;
    public bool FacingRight { get; protected set; } = true;

    // to flip health bar
    public Action onFlipped;

    #region Components
    public Rigidbody2D Rb { get; protected set; }
    public virtual EntityFX Fx { get; protected set; }
    public Animator Anim { get; protected set; }
    public SpriteRenderer Sr { get; protected set; }
    public CharStats Stats { get; protected set; }
    public Collider2D Cd { get; protected set; }

    #endregion

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Fx = GetComponent<EntityFX>();
        Anim = GetComponentInChildren<Animator>();
        Sr = GetComponentInChildren<SpriteRenderer>();
        Stats = GetComponent<CharStats>();
        Cd = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void DamageImpact()
    {
        Fx.StartCoroutine(nameof(EntityFX.FlashFXCoroutine));
        StartCoroutine(HitKnockbackCoroutine());
    }

    public virtual void SetupKnockbackDirection(Transform damageDir)
    {
        if (damageDir.position.x > transform.position.x)
            KnockbackDir = -1;
        else if (damageDir.position.x < transform.position.x)
            KnockbackDir = 1;
    }

    public virtual void SetupKnockbackVector(Vector2 v) => knockbackVector = v;

    // idk how and why does it work on enemies and does not on player, why do I need duration here etc
    protected virtual IEnumerator HitKnockbackCoroutine()
    {
        isKnocked = true;
        Rb.velocity = new Vector2(knockbackVector.x * KnockbackDir, knockbackVector.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    public abstract void SlowBy(float slowPercentage, float slowDuration);
    protected virtual void RestoreBaseSpeed() => Anim.speed = 1f;

    public abstract void Die();

    #region Velocity
    public virtual void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked) // WHY
            return;

        Rb.velocity = new Vector2(xVelocity, yVelocity);
        CheckFlip(xVelocity);
    }

    public virtual void SetZeroVelocity() => SetVelocity(0, 0);
    #endregion

    #region Collision
    public virtual bool IsGroundDetected =>
        Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected =>
        Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        FacingDirection = -1 * FacingDirection;
        FacingRight = !FacingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    // flip is checked in SetVelocity by default
    // if SetVelocity is called in Update, flip is being checked every tick too
    public virtual void CheckFlip(float x)
    {
        if (x > 0 && !FacingRight
         || x < 0 && FacingRight)
            Flip();
    }
    #endregion
}