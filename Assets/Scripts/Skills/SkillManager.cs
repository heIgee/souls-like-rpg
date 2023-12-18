using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public DashSkill Dash { get; private set; }
    public CloneSkill Clone { get; private set; }
    public SwordSkill Sword { get; private set; }
    public BlackHoleSkill BlackHole { get; private set; }
    public CrystalSkill Crystal { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        Dash = GetComponent<DashSkill>();
        Clone = GetComponent<CloneSkill>();
        Sword = GetComponent<SwordSkill>();
        BlackHole = GetComponent<BlackHoleSkill>();
        Crystal = GetComponent<CrystalSkill>();
    }
}
