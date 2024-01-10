using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency = 0;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool AttemptBuySkill(int price)
    {
        if (currency < price)
        {
            Debug.LogWarning($"Insifficient currency. You have {currency}, need {price}");
            return false;
        }

        currency -= price;
        return true;
    }

    public void LoadData(GameData data)
    {
        currency = data.currency;
    }

    public void SaveData(GameData data)
    {
        data.currency = currency;
    }
}
