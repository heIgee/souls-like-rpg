using Unity.VisualScripting;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;    

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;

    public CraftWindowUI craftWindowUI;

    public ItemTooltipUI itemTooltip;
    public StatTooltipUI statTooltip;

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
        CloseAllMenus();

        SwitchTo(craftUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchTo(characterUI);

        if (Input.GetKeyDown(KeyCode.V))
            SwitchTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchTo(craftUI);

        if (Input.GetKeyDown(KeyCode.N))
            SwitchTo(optionsUI);
    }

    public void SwitchTo(GameObject menu)
    {
        if (menu != null && menu.activeSelf)
        {
            menu.SetActive(false);
            return;
        }

        CloseAllMenus();

        menu.SetActive(true);
    }

    private void CloseAllMenus()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
