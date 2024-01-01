using UnityEngine;

public class CrystalController : MonoBehaviour
{
    private Animator Anim => GetComponentInChildren<Animator>();

    private float crystalTimer;

    [Header("Moving crystal")]
    private bool canMove;
    private float moveSpeed;

    [Header("Explosive crystal")]
    private bool canExplode;

    private bool canGrow;
    private readonly float growSpeed = 5f;

    private Transform closestTarget;
    // this could be done in loop or component check, we use mask this time
    [SerializeField] private LayerMask whatIsEnemy;

    public void ExplosionDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
                PlayerManager.instance.player.Stats.DoMagicalDamage(hit.GetComponent<EnemyStats>(), includeAmulet:true);
    }

    public void ChooseRandomTarget()
    {
        float radius = SkillManager.instance.BlackHole.maxSize / 2;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    public void SetupCrystal(float crystalDuration, bool canExplode, bool canMove, float moveSpeed)
    {
        crystalTimer = crystalDuration;
        this.canExplode = canExplode;
        this.canMove = canMove;
        this.moveSpeed = moveSpeed;
    }

    private void Update()
    {
        crystalTimer -= Time.deltaTime;

        if (crystalTimer < 0)
            FinishCrystal();


        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3),
                growSpeed * Time.deltaTime);

        if (canMove)
        {
            if (closestTarget == null && !Skill.TryGetNearestEnemy(transform, out closestTarget))
                return;

            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position,
                moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1f)
            { 
                FinishCrystal();
                canMove = false;
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            Anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
