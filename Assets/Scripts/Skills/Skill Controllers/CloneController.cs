using UnityEngine;

public class CloneController : MonoBehaviour
{
    // GetComponent in Awake does not work
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 0.8f;

    private bool canDuplicateClone;
    private float duplicateChance;

    private int facingDir = 1;

    private CloneSkill skill;

    private void Awake()
    {
        // does not work
        //sr = GetComponent<SpriteRenderer>();
        //anim = GetComponent<Animator>();
        skill = SkillManager.instance.Clone;
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

        if (sr.color.a < 0)
            Destroy(gameObject);
    }

    public void SetupClone(Transform cloneTransform, Vector3 offset, float cloneDuration)
    {
        anim.SetInteger("AttackNumber", Random.Range(1, 4));

        transform.position = cloneTransform.position + offset;
        cloneTimer = cloneDuration;

        canDuplicateClone = skill.duplicateCloneUnlocked;
        duplicateChance = skill.duplicateChance;

        FaceClosestTarget();
    }

    public void SetupClone(Transform cloneTransform, float cloneDuration)
    {
        anim.SetInteger("AttackNumber", Random.Range(1, 3));

        transform.position = cloneTransform.position;
        cloneTimer = cloneDuration;

        canDuplicateClone = skill.duplicateCloneUnlocked;
        duplicateChance = skill.duplicateChance;

        FaceClosestTarget();
    }

    private void FaceClosestTarget()
    {
        if (!Skill.TryGetNearestEnemy(attackCheck, out Transform closestTarget))
            return;

        if (transform.position.x > closestTarget.position.x)
        {
            facingDir = -1;
            transform.Rotate(0, 180, 0);
        }
    }

    private void AnimationTrigger() 
    {
        cloneTimer = -0.1f; // just negative value to make clone disappear
        anim.speed = 0f; // freeze after first attack
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats targetStats = hit.GetComponent<EnemyStats>();
                PlayerStats playerStats = PlayerManager.instance.player.Stats as PlayerStats;

                playerStats.DoCloneDamage(targetStats, skill.attackMultiplier, includeAmulet: true);

                if (skill.aggresiveCloneUnlocked)
                {
                    if (Inventory.instance.TryGetEquipment(EquipmentType.Weapon, out var equippedItem))
                        equippedItem.ExecuteEffects(hit.transform);
                }

                playerStats.DoPhysicalDamage(targetStats);
                        
                // clone duplication
                if (canDuplicateClone && Random.Range(0, 100) < duplicateChance)
                    SkillManager.instance.Clone.CreateClone(hit.transform, new Vector3(1f * facingDir, 0));
            }
    }
}
