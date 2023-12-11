using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Awake()
    {
        // does not work
        //sr = GetComponent<SpriteRenderer>();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0) // I don't understand this check
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

        if (sr.color.a < 0)
            Destroy(gameObject);
    }

    public void SetupClone(Transform cloneTransform, float cloneDuration, bool canAttack)
    {
        if (anim == null)
            Debug.Log("Anim is null. Kys");

        if (canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        transform.position = cloneTransform.position;
        cloneTimer = cloneDuration;

        FaceClosestTarget();
    }

    private void FaceClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, 10);

        if (colliders.Length <= 0)
            return;

        var closestTarget = colliders.Where(hit => hit.GetComponent<Enemy>() != null)
            .OrderBy(hit => Vector2.Distance(transform.position, hit.transform.position))
            .FirstOrDefault();

        if (closestTarget != null && transform.position.x > closestTarget.transform.position.x)
            transform.Rotate(0, 180, 0);
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
                hit.GetComponent<Enemy>().Damage();
    }
}
