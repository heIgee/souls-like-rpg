using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class CharStats : MonoBehaviour
{
    protected EntityFX fx;
    protected Entity holder;

    [Header("Major stats")]
    public Stat strength; // +1 dmg and +1 crit dmg
    public Stat agility; // +1 evasion and +1 crit chance
    public Stat intelligence; // +1 magic dmg and +3 magic resistance
    public Stat vitality; // +3 health

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

    public Stat totalMagicDamage;

    public int TotalMagicDamage => fireDamage.Value + iceDamage.Value + lightningDamage.Value + intelligence.Value;

    public static int RandPercent => Random.Range(0, 100);
    public static bool CoinFlip => Random.Range(0, 2) == 0;

    public bool isIgnited; // does dmg over time
    public bool isChilled; // decrease armor by % and slows 
    public bool isShocked; // decrease accuracy by %

    [SerializeField] private float ailmentDuration = 3f;

    protected float ignitedTimer;
    protected float chilledTimer;
    protected float shockedTimer;

    protected float igniteDamageCooldown = 0.3f;
    protected float igniteDamageTimer;
    protected int igniteDamage;

    [SerializeField] protected GameObject shockStrikePrefab;
    protected int thunderDamage;


    public System.Action onHealthChanged;

    public int CurrentHp { get; protected set; }
    public bool IsDead { get; private set; }

    // script execution order set this Start to be first before HealthBarUI Start
    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        holder = GetComponent<Entity>();

        totalMagicDamage.SetBaseValue(fireDamage.Value + iceDamage.Value + lightningDamage.Value);
        critDamage.SetBaseValue(150);
        CurrentHp = maxHp.Value;

        ApplyMajorStatsModifiers();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnited)
            TakeIgniteDamage();
    }

    public virtual void BuffStat(Stat stat, int modifier, float duration)
    {
        if (modifier <= 0 || duration <= 0 || stat == null)
            return;

        StartCoroutine(BuffCoroutine(stat, modifier, duration));
    }

    private IEnumerator BuffCoroutine(Stat stat, int modifier, float duration)
    {
        stat.AddModifier(modifier);
        yield return new WaitForSeconds(duration);
        stat.RemoveModifier(modifier);
    }

    protected virtual void ApplyMajorStatsModifiers()
    {
        // strength: +1 dmg and +1 crit dmg
        // agility: +1 evasion and +1 crit chance
        // intelligence: +1 magic dmg and +3 magic resistance
        // vitality: +3 health

        // strength
        damage.AddModifier(strength.Value);
        critDamage.AddModifier(strength.Value);

        // agility
        evasion.AddModifier(agility.Value);
        critChance.AddModifier(agility.Value);

        // intelligence
        // applying TotalMagicDamage buff in its property getter
        magicRes.AddModifier(intelligence.Value * 3);

        // vitality
        maxHp.AddModifier(vitality.Value * 3);
    }

    public virtual void DoPhysicalDamage(CharStats target, bool includeAmulet = false)
    {
        if (includeAmulet && Inventory.instance.TryGetEquipment(EquipmentType.Amulet, out var amulet))
        {
            Debug.LogWarning(amulet);
            amulet.ExecuteEffects(target.transform);
        }

        if (AttemptAvoid(target))
            return;

        int totalPhysDamage = damage.Value;

        if (AttemptCrit())
        {
            totalPhysDamage = CalculateCrit(totalPhysDamage);
            Debug.LogWarning($"{gameObject.name} performed CRIT [{totalPhysDamage}]" +
                $" on {target.gameObject.name}");
        }

        if (target.isChilled)
            totalPhysDamage -= (int)(target.armor.Value * 0.7f);
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
            Ailment.Fire => () => target.SetIgniteDamage(Mathf.RoundToInt(fireDmg * 0.2f)),
            Ailment.Shock => () => target.SetThunderStrikeDamage(Mathf.RoundToInt(lightningDmg * 0.1f)),
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

    private void ApplyFire()
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

    private void ApplyChill()
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
        thunder.GetComponent<ShockStrikeController>().Setup(thunderDamage, closestTarget.GetComponent<CharStats>());
    }

    public void ApplyShock()
    {
        shockedTimer = ailmentDuration;
        isShocked = true;

        fx.RunShockFXFor(ailmentDuration);
    }
    private void TakeIgniteDamage()
    {
        DecreaseHealth(igniteDamage);

        if (CurrentHp < 0 && !IsDead)
            Die();

        igniteDamageTimer = igniteDamageCooldown;
    }

    public void SetIgniteDamage(int damage) => igniteDamage = damage;

    public void SetThunderStrikeDamage(int damage) => thunderDamage = damage;

    #endregion

    private bool AttemptAvoid(CharStats target)
    {
        int totalEvasion = target.evasion.Value;

        if (target.isShocked)
            totalEvasion += 20;

        if (RandPercent < totalEvasion)
        {
            Debug.LogWarning($"{gameObject.name} AVOIDED attack from" +
                $" {target.gameObject.name}");
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
        CurrentHp -= damage;
        onHealthChanged?.Invoke();
    }

    public virtual void IncreaseHealth(int damage)
    {
        CurrentHp += damage;

        if (CurrentHp > maxHp.Value)
            CurrentHp = maxHp.Value;

        onHealthChanged?.Invoke();
    }

    private bool AttemptCrit() => RandPercent < critChance.Value;


    private int CalculateCrit(int damage)
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
