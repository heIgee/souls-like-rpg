using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
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

        SwitchTo(inGameUI);

        fadeScreen.gameObject.SetActive(true);
        wastedText.gameObject.SetActive(false);
        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
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

    public void SwitchTo(GameObject menu)
    {
        CloseAllMenus();
        menu.SetActive(true);
    }

    public void KeySwitchTo(GameObject menu)
    { 
        if (menu != null && menu.activeSelf)
        {
            menu.SetActive(false);
            inGameUI.SetActive(true);
            return;
        }

        CloseAllMenus();
        menu.SetActive(true);
    }

    private void CloseAllMenus()
    {
        foreach (GameObject menu in menus)
            menu.SetActive(false);

        // temp
        //inGameUI.SetActive(true);
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
}
