using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    public static UI instance;

    [Header("End screen")]
    [SerializeField] private FadeScreenUI fadeScreen;
    [SerializeField] private GameObject wastedText;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;

    private List<GameObject> menus = new();

    [SerializeField] private GameObject inGameUI;

    public CraftWindowUI craftWindowUI;

    public ItemTooltipUI itemTooltip;
    public StatTooltipUI statTooltip;
    public SkillTooltipUI skillTooltip;

    public Sprite defaultIconSprite;

    [SerializeField] private VolumeSliderUI[] volumeSliders;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        menus = new List<GameObject>()
        {
            characterUI,
            skillTreeUI,
            craftUI,
            optionsUI,
            inGameUI
        };

        CloseAllMenus();

        SwitchToInGameUI();

        fadeScreen.gameObject.SetActive(true);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
        wastedText.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SwitchToEndScreen();

        if (Input.GetKeyDown(KeyCode.C))
            KeySwitchTo(characterUI);

        if (Input.GetKeyDown(KeyCode.V))
            KeySwitchTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.B))
            KeySwitchTo(craftUI);

        if (Input.GetKeyDown(KeyCode.N))
            KeySwitchTo(optionsUI);

        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchTo(inGameUI);
    }

    private void SwitchToInGameUI()
    {
        CloseAllMenus();
        inGameUI.SetActive(true);
    }

    // renaming this breaks all menu header buttons fyi
    public void SwitchTo(GameObject menu)
    {
        CloseAllMenus();
        menu.SetActive(true);
        AudioManager.instance.PlaySFX(7); // dzing
    }

    public void KeySwitchTo(GameObject menu) // closes if active
    { 
        if (menu != null && menu.activeSelf)
        {
            menu.SetActive(false);
            SwitchToInGameUI();
            return;
        }

        SwitchTo(menu);
    }

    private void CloseAllMenus()
    {
        foreach (GameObject menu in menus)
            menu.SetActive(false);
    }

    public void SwitchToEndScreen()
    {
        StartCoroutine(EndSreenCoroutine(2f));
    }

    private IEnumerator EndSreenCoroutine(float delay)
    {
        CloseAllMenus();

        fadeScreen.FadeInDelay(delay);
        yield return new WaitForSeconds(3f);
        wastedText.SetActive(true);
    }

    public void LoadData(GameData data)
    {
        foreach (KeyValuePair<string, float> kvp in data.volumeSettings)
            if (volumeSliders.Any(s => s.parameter == kvp.Key))
             volumeSliders.First(s => s.parameter == kvp.Key)
                .LoadSlider(kvp.Value);
    } 

    public void SaveData(GameData data)
    {
        data.volumeSettings.Clear();

        foreach (var slider in volumeSliders)
            data.volumeSettings.Add(slider.parameter, slider.slider.value);
    }
}
