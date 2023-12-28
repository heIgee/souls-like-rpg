using System.Linq;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    [SerializeField] protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public bool AttemptUse()
    {
        if (cooldownTimer < 0)
        {
            Use();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.LogWarning($"Skill {GetType().Name} is on cooldown");
        return false;
    }

    public abstract void Use();
    
    public static bool TryGetNearestEnemy(Transform checkTransform, out Transform nearestEnemy)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkTransform.position, 10f);

        if (colliders.Length <= 0)
        {
            nearestEnemy = null;
            return false;
        }

        var closestEnemy = colliders
            // get only enemies excluding the one from which method finds targets
            .Where(hit => hit.GetComponent<Enemy>() != null && hit.transform != checkTransform)
            .OrderBy(hit => Vector2.Distance(checkTransform.position, hit.transform.position))
            .FirstOrDefault();

        if (closestEnemy != null)
        {
            nearestEnemy = closestEnemy.transform;
            return true;
        }
        else
        {
            nearestEnemy = null;
            return false;
        }
    }

}
