using UnityEngine;

public class CraftSetupUI : MonoBehaviour
{
    [SerializeField] private Transform defaultSlot;

    private void Start()
    {
        defaultSlot.GetComponent<CraftTypeUI>().SetupCraftList();
    }
}
