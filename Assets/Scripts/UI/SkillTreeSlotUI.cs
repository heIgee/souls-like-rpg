using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    [SerializeField] private bool isUnlocked;
    public bool IsUnlocked
    {
        get
        {
            CheckColor();
            return isUnlocked;
        }
        set
        {
            isUnlocked = value;
            CheckColor();
        }
    }

    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private int skillPrice = 0;

    [SerializeField] private SkillTreeSlotUI[] preliminaries;
    [SerializeField] private SkillTreeSlotUI[] blockers;

    [SerializeField] protected Image skillImage;
    [SerializeField] protected Color lockedSkillColor;


    private void OnValidate()
    {
        gameObject.name = $"Skill slot: {skillName}";
        skillImage = GetComponent<Image>();
        CheckColor();

        GetComponent<Button>().onClick.AddListener(() => {
            UnlockSkill(); 
        });
    }

    private void Start()
    {
        CheckColor();
    }

    private void CheckColor()
    {
        if (isUnlocked)
            ChangeColorToUnlocked();
        else
            ChangeColorToLocked();
    }

    private void ChangeColorToLocked()
    {
        skillImage.color = lockedSkillColor;
    }

    public void ChangeColorToUnlocked()
    {
        skillImage.color = Color.white;
    }

    public void UnlockSkill()
    {
        if (IsUnlocked)
            return;

        foreach (var skill in preliminaries)
            if (!skill.IsUnlocked)
            {
                Debug.LogWarning("Not all preliminaries are unlocked");
                return;
            }

        foreach (var skill in blockers)
            if (skill.IsUnlocked)
            {
                Debug.LogWarning("Some blockers are unlocked");
                return;
            }

        if (!PlayerManager.instance.AttemptBuySkill(skillPrice))
            return;

        IsUnlocked = true;
        ChangeColorToUnlocked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.instance.skillTooltip.ShowToolTip(skillName, skillDescription, skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogWarning(UI.instance);
        //Debug.LogWarning(UI.instance.skillTooltip);
        //Debug.LogWarning(nameof(UI.instance.skillTooltip.HideTooltip));

        // lil bit of delay to not blink
        UI.instance.skillTooltip.HideTooltipDelay(0.4f);
    }

    public void LoadData(GameData data)
    {
        if (data.skills.TryGetValue(skillName, out bool unlocked))
            IsUnlocked = unlocked;
    }

    public void SaveData(GameData data)
    {
        if (data.skills.TryGetValue(skillName, out _))
            data.skills.Remove(skillName);

        data.skills.Add(skillName, IsUnlocked);
    }
}
