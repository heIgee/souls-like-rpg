using UnityEngine;

public abstract class CharStats : MonoBehaviour
{

    [Header("Major stats")]
    public Stat strength; // +1 dmg and +1 crit dmg
    public Stat agility; // +1 evasion and +1 crit chance
    public Stat intelligence; // +1 magic dmg and +1 magic resistance
    public Stat vitality; // +3 hp

    [Header("Defensive stats")]
    public Stat maxHp;
    public Stat armor;
    public Stat evasion;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critDamage;  // default 150%


    [SerializeField] private int currentHp;

    protected virtual void Start()
    {
        critDamage.SetDefault(150);
        currentHp = maxHp.Value;
    }

    public virtual void DoDamage(CharStats target)
    {
        if (AttemptAvoid(target))
            return;

        int totalDamage = damage.Value + strength.Value;

        totalDamage -= target.armor.Value;

        if (AttemptCrit())
        {
            totalDamage = CalculateCrit(totalDamage);
            Debug.LogWarning("CRIT: " + totalDamage);
        }

        if (totalDamage < 0)
            totalDamage = 0;

        target.TakeDamage(totalDamage);
    }

    private static bool AttemptAvoid(CharStats target)
    {
        int totalEvasion = target.evasion.Value + target.agility.Value;

        if (Random.Range(0, 99) < totalEvasion)
        {
            Debug.LogWarning("Avoided!");
            return true;
        }
        return false;
        //return Random.Range(0, 99) < totalEvasion;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0)
            Die();
    }

    private bool AttemptCrit()
    {
        int totalChance = critChance.Value + agility.Value;

        if (Random.Range(0, 99) < totalChance)
        { 
            return true;
        }

        return false;
    }

    private int CalculateCrit(int damage)
    {
        float totalCrit = (critDamage.Value + strength.Value) / 100f;
        float critHit = damage * totalCrit;
        return Mathf.RoundToInt(critHit);
    }

    protected abstract void Die();
}
