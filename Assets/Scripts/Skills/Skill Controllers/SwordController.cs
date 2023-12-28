using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private SwordType swordType;
    private float freezeTimeDuration;

    private Player player;

    private Rigidbody2D rb;
    private Collider2D cd;

    private Animator anim;
    private SpriteRenderer sr;

    private float returnSpeed = 15f;
    private bool isReturning;

    private bool canRotate = true;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount = 0;

    [Header("Bounce info")]
    private float bounceSpeed = 20f;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;

    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;


    private bool canBounce = false;
    private int bounceAmount = 4;

    // Unity inits Lists automatically only when they're public
    private List<Transform> targets = new();
    private int targetIndex;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<Collider2D>();

        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        Debug.Log("Anim: " + anim);
        Debug.Log("Rb: " + rb);
        Debug.Log("Cd: " + cd);
    }

    private void Start()
    {
        // this shouldn't be in Awake cause instance may not be initialized at that time
        player = PlayerManager.instance.player;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
            UpdateReturn();
        else if (canBounce && targets.Count > 0)
            UpdateBounce();
        else if (isSpinning)
            UpdateSpin();
    }


    private void UpdateReturn()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position,
            returnSpeed * Time.deltaTime);

        // gpt rocks
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;

        // set the rotation so the sword handle faces the player
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Vector2.Distance(transform.position, player.transform.position) < 1f)
            player.CatchSword();
    }

    private void UpdateBounce()
    {
        transform.position = Vector2.MoveTowards(transform.position, targets[targetIndex].position,
            bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targets[targetIndex].position) < 0.1f)
        {
            SwordSkillDamage(targets[targetIndex].GetComponent<Enemy>());

            targetIndex++;
            bounceAmount--;

            if (bounceAmount <= 0)
            {
                canBounce = false;
                isReturning = true;
            }
        }

        if (targetIndex >= targets.Count)
            targetIndex = 0;
    }

    private void UpdateSpin()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            FreezeWhileSpinning();

        if (wasStopped)
        {
            spinTimer -= Time.deltaTime;

            if (spinTimer < 0)
            {
                isSpinning = false;
                isReturning = true;
            }

            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

            hitTimer -= Time.deltaTime;

            if (hitTimer < 0)
                hitTimer = hitCooldown;

            Collider2D[] impactedColliders = GetColliderOverlap(1);

            foreach (var hit in impactedColliders)
                if (hit.TryGetComponent<Enemy>(out var Enemy))
                    SwordSkillDamage(Enemy);
              
        }
}

    private void FreezeWhileSpinning()
    {
        spinTimer = spinDuration;
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void SetupBounce(bool canBounce, int bounceAmount, float bounceSpeed)
    {
        this.canBounce = canBounce;
        this.bounceAmount = bounceAmount;
        this.bounceSpeed = bounceSpeed;
    }

    public void SetupPierce(int pierceAmount)
    {
        this.pierceAmount = pierceAmount;
    }

    public void SetupSpin(bool isSpinning, float maxTravelDistance, float spinDuration, float hitCooldown)
    {
        this.isSpinning = isSpinning;
        this.maxTravelDistance = maxTravelDistance;
        this.spinDuration = spinDuration;
        this.hitCooldown = hitCooldown;
    }

    public void SetupSword(SwordType swordType, Vector2 dir, float gravityScale, 
        float freezeTimeDuration, float returnSpeed)
    {
        this.swordType = swordType;
        this.freezeTimeDuration = freezeTimeDuration;
        this.returnSpeed = returnSpeed;

        rb.velocity = dir;
        rb.gravityScale = gravityScale;

        if (swordType != SwordType.Pierce)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);
    }

    public void ReturnSword()
    {
        // too far away, destroying returns it immediately
        if (Vector2.Distance(transform.position, player.transform.position) >= 200f)
            Destroy(gameObject);

        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        transform.parent = null;
        isReturning = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        //Debug.Log("Entered OnTriggerEnter2D in " + GetType().Name + "\nCollision: " + collision);

        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            SwordSkillDamage(enemy);

            if (canBounce && targets.Count <= 0)
            {
                Collider2D[] detectedColliders = GetColliderOverlap(10);

                foreach (var hit in detectedColliders)
                    if (hit.GetComponent<Enemy>() != null)
                        targets.Add(hit.transform);
            }
        }

        CollisionBehaviour(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        player.Stats.DoPhysicalDamage(enemy.Stats, includeAmulet:true);

        enemy.FreezeTimeFor(freezeTimeDuration);
    }

    private Collider2D[] GetColliderOverlap(float radius) => cd switch
    {
        CircleCollider2D => Physics2D.OverlapCircleAll(transform.position, radius),
        BoxCollider2D => Physics2D.OverlapBoxAll(transform.position, new Vector2(radius, radius), 0f),
        _ => throw new NotSupportedException("Collider is not a circle nor box. " +
                "Cannot calculate overlaps")
    };

    private void CollisionBehaviour(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            if (!wasStopped)
                FreezeWhileSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;


        if (canBounce && targets.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }


}

