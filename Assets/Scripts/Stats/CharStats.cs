using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharStats : MonoBehaviour
{
    protected EntityFX fx;
    protected Entity holder;

    [Header("Major stats")]
    public MajorStat strength; // +1 dmg and +1 crit dmg
    public MajorStat agility; // +1 evasion and +1 crit chance
    public MajorStat intelligence; // +1 magic dmg and +3 magic resistance
    public MajorStat vitality; // +3 health

    public int CurrentHp;/* { get; protected set; }*/

    [Header("Defensive stats")]
    public Stat maxHp;
    public Stat armor;
    public Stat evasion;
    public Stat magicRes;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critDamage;  // default 150%

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Header("Move stats TODO")]
    //public Stat moveSpeed; 
    // ...


    public Stat totalMagicDamage;

    public int TotalMagicDamage => fireDamage.Value + iceDamage.Value + lightningDamage.Value + intelligence.Value;

    public static int RandPercent => Random.Range(0, 100);
    public static bool CoinFlip => Random.Range(0, 2) == 0;

    public bool isIgnited; // does dmg over time
    public bool isChilled; // decrease armor by % and slows 
    public bool isShocked; // decrease accuracy by %

    protected const float chilledArmorDebuff = 0.7f;

    [SerializeField] protected float ailmentDuration = 3f;

    protected float ignitedTimer;
    protected float chilledTimer;
    protected float shockedTimer;

    protected float ignitedDamageCooldown = 0.3f;
    protected float ignitedDamageTimer;
    protected int ignitedDamage;

    [SerializeField] protected GameObject shockStrikePrefab;
    protected int thunderStruckDamage;


    public System.Action onHealthChanged;

    public bool IsDead { get; protected set; }

    [SerializeField] protected float vulnerabilityDamageModifier = 1.5f;
    protected bool isVulnerable;

    // script execution order set this Start to be first before HealthBarUI Start
    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        holder = GetComponent<Entity>();

        totalMagicDamage.SetBaseValue(fireDamage.Value + iceDamage.Value + lightningDamage.Value);
        critDamage.SetBaseValue(150);
        CurrentHp = maxHp.Value;

        BindAffectedStats();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        ignitedDamageTimer -= Time.deltaTime;

        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (ignitedDamageTimer < 0 && isIgnited)
            TakeIgniteDamage();
    }

    protected virtual void BindAffectedStats()
    {
        // strength: +1 dmg and +1 crit dmg
        // agility: +1 evasion and +1 crit chance
        // intelligence: +1 magic dmg and +3 magic res
        // vitality: +3 health

        strength.AddAffectedStat(damage, 1);
        strength.AddAffectedStat(critDamage, 1);

        agility.AddAffectedStat(evasion, 1);
        agility.AddAffectedStat(critChance, 1);

        // applying TotalMagicDamage buff in its property getter
        // (logic is not completed)
        intelligence.AddAffectedStat(magicRes, 3);

        vitality.AddAffectedStat(maxHp, 3);

    }

    public void MakeVulnerableFor(float seconds) => StartCoroutine(VulnerabilityCoroutine(seconds));

    protected IEnumerator VulnerabilityCoroutine(float seconds)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(seconds);
        isVulnerable = false;
    }

    public virtual void BuffStat(Stat stat, int modifier, float duration)
    {
        if (modifier <= 0 || duration <= 0 || stat == null)
            return;

        StartCoroutine(BuffCoroutine(stat, modifier, duration));
    }

    protected IEnumerator BuffCoroutine(Stat stat, int modifier, float duration)
    {
        stat.AddModifier(modifier);
        yield return new WaitForSeconds(duration);
        stat.RemoveModifier(modifier);
    }

    public virtual void DoPhysicalDamage(CharStats target, bool includeAmulet = false)
    {
        if (includeAmulet && Inventory.instance.TryGetEquipment(EquipmentType.Amulet, out var amulet))
        {
            Debug.LogWarning(amulet);
            amulet.ExecuteEffects(target.transform);
        }

        if (target.AttemptAvoid(this))
            return;

        int totalPhysDamage = damage.Value;

        if (AttemptCrit())
        {
            totalPhysDamage = CalculateCrit(totalPhysDamage);
            Debug.LogWarning($"{gameObject.name} performed CRIT [{totalPhysDamage}]" +
                $" on {target.gameObject.name}");
        }

        if (target.isChilled)
            totalPhysDamage -= Mathf.RoundToInt(target.armor.Value * chilledArmorDebuff);
        else
            totalPhysDamage -= target.armor.Value;

        if (totalPhysDamage < 0)
            totalPhysDamage = 0;

        target.TakeDamage(totalPhysDamage);
    }

    #region Magic damage & ailments

    public virtual void DoMagicalDamage(CharStats target, bool includeAmulet = false)
    {
        if (includeAmulet && Inventory.instance.TryGetEquipment(EquipmentType.Amulet, out var amulet))
        {
            Debug.LogWarning(amulet);
            amulet.ExecuteEffects(target.transform);
        }

        int fireDmg = fireDamage.Value;
        int iceDmg = iceDamage.Value;
        int lightningDmg = lightningDamage.Value;

        int totalMagicDmg = TotalMagicDamage;

        totalMagicDmg -= target.magicRes.Value;

        if (totalMagicDmg < 0)
            totalMagicDmg = 0;

        target.TakeDamage(totalMagicDmg);

        // holy hell that shit, man, its barely working
        //bool canIgnite = fireDmg > iceDmg && fireDmg > lightningDmg;
        //bool canChill = iceDmg > fireDmg && iceDmg > lightningDmg;
        //bool canShock = lightningDmg > fireDmg && lightningDmg > iceDmg;

        // this one is better
        List<(float damage, Ailment ailment)> ailmentCandidates = new()
        {
            (fireDmg, Ailment.Fire),
            (iceDmg, Ailment.Ice),
            (lightningDmg, Ailment.Shock)
        };

        // remove candidates without damage values
        ailmentCandidates.RemoveAll(candidate => candidate.damage <= 0);

        if (ailmentCandidates.Count <= 0)
            return; // no magic damage to apply

        // sort in descending order
        ailmentCandidates.Sort((a, b) => b.damage.CompareTo(a.damage));

        ailmentCandidates.RemoveAll(candidate => candidate.damage < ailmentCandidates[0].damage);

        // choose random ailment if some values are equal
        Ailment chosenAilment = ailmentCandidates[Random.Range(0, ailmentCandidates.Count)].ailment;

        // this is literally the most masterpiecest code I've ever written
        System.Action _ = chosenAilment switch
        {
            Ailment.Fire => () => target.SetIgnitedDamage(Mathf.RoundToInt(fireDmg * 0.2f)),
            Ailment.Shock => () => target.SetThunderStruckDamage(Mathf.RoundToInt(lightningDmg * 0.1f)),
            _ => () => { }
        };

        _();

        // and just some ordinary peasantish switch-case
        //switch (chosenAilment)
        //{
        //    case Ailment.Fire:
        //        target.SetIgniteDamage(Mathf.RoundToInt(fireDmg * 0.2f));
        //        break;

        //    case Ailment.Shock:
        //        target.SetThunderStrikeDamage(Mathf.RoundToInt(lightningDmg * 0.1f));
        //        break;

        //    default:
        //        break;
        //}

        target.ApplyAilment(chosenAilment);
    }

    public virtual void ApplyAilment(Ailment ailment)
    {
        //if (isIgnited || isChilled)
        //    return;

        // currently, new ailment applies even if entity is affected by the old one
        // see details in each method
        System.Action _ = ailment switch
        {
            Ailment.Fire => ApplyFire,
            Ailment.Ice => ApplyChill,
            Ailment.Shock =>
                isShocked && this is not PlayerStats
                ? ReleaseShockStrike
                : ApplyShock,
            _ => () => { }
        }; 

        _();
    }

    protected void ApplyFire()
    {
        if (isChilled)
        {
            isChilled = false;
            return;
        }

        isIgnited = true;
        ignitedTimer = ailmentDuration;

        fx.RunIgniteFXFor(ailmentDuration);
    }

    protected void ApplyChill()
    {
        if (isIgnited)
        {
            isIgnited = false;
            return;
        }

        isChilled = true;
        chilledTimer = ailmentDuration;

        fx.RunChillFXFor(ailmentDuration);

        float slowPercentage = 0.2f;
        holder.SlowBy(slowPercentage, ailmentDuration);
    }

    public void ReleaseShockStrike()
    {
        // so genius
        if (!Skill.TryGetNearestEnemy(transform, out Transform closestTarget))
            closestTarget = transform;

        //Debug.LogWarning("Closest target: " + closestTarget);

        GameObject thunder = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
        thunder.GetComponent<ShockStrikeController>().Setup(thunderStruckDamage, closestTarget.GetComponent<CharStats>());
    }

    public void ApplyShock()
    {
        shockedTimer = ailmentDuration;
        isShocked = true;

        fx.RunShockFXFor(ailmentDuration);
    }
    protected void TakeIgniteDamage()
    {
        DecreaseHealth(ignitedDamage);

        if (CurrentHp < 0 && !IsDead)
            Die();

        ignitedDamageTimer = ignitedDamageCooldown;
    }

    public void SetIgnitedDamage(int damage) => ignitedDamage = damage;

    public void SetThunderStruckDamage(int damage) => thunderStruckDamage = damage;

    #endregion

    public virtual void OnEvade(Transform spawnTransform)
    {
    }

    public bool AttemptAvoid(CharStats attacker)
    {
        int totalEvasion = evasion.Value;

        if (isShocked)
            totalEvasion -= 20;

        if (RandPercent < totalEvasion)
        {
            Debug.LogWarning($"{gameObject.name} AVOIDED attack from " +
                $"{attacker.gameObject.name}");
            OnEvade(attacker.transform);
            return true;
        }
        return false;
        //return Random.Range(0, 99) < totalEvasion;
    }

    public virtual void TakeDamage(int damage)
    {
        DecreaseHealth(damage);

        holder.DamageImpact();

        if (CurrentHp < 0 && !IsDead)
            Die();
    }

    protected virtual void DecreaseHealth(int damage)
    {
        if (isVulnerable)
            CurrentHp -= Mathf.RoundToInt(damage * vulnerabilityDamageModifier);
        else
            CurrentHp -= damage;

        onHealthChanged?.Invoke();
    }

    public virtual void IncreaseHealth(int heal)
    {
        CurrentHp += heal;

        if (CurrentHp > maxHp.Value)
            CurrentHp = maxHp.Value;

        onHealthChanged?.Invoke();
    }

    protected bool AttemptCrit() => RandPercent < critChance.Value;


    protected int CalculateCrit(int damage)
    {
        float critPercentage = critDamage.Value / 100f;
        float critHit = damage * critPercentage;
        return Mathf.RoundToInt(critHit);
    }

    protected virtual void Die()
    {
        holder.Die();
        IsDead = true;
    }
}
