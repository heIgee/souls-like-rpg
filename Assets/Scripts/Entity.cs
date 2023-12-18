using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
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
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;

    public int FacingDirection { get; protected set; } = 1;
    public bool FacingRight { get; protected set; } = true;

    #region Components
    public Rigidbody2D Rb { get; protected set; }
    public EntityFX Fx { get; protected set; }
    public Animator Anim { get; protected set; }
    public SpriteRenderer Sr { get; protected set; }

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
    }

    protected virtual void Update()
    {

    }

    public virtual void Damage()
    {
        Fx.StartCoroutine(nameof(EntityFX.FlashFX)); 
        StartCoroutine(nameof(HitKnockback));
        //Debug.Log(gameObject.name + " was damaged");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        Rb.velocity = new Vector2(knockbackDirection.x * -FacingDirection, knockbackDirection.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    #region Velocity
    public virtual void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
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
    }

    // flip is checked in SetVelocity by default
    // if SetVelocity is called in Update, flip is checked every tick too
    public virtual void CheckFlip(float x)
    {
        if (x > 0 && !FacingRight
         || x < 0 && FacingRight)
            Flip();
    }
    #endregion

    public void SetTransparency(bool _) => Sr.color = _ ? Color.clear : Color.white;
}
