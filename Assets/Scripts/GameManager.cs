using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    [SerializeField] private Checkpoint[] checkpoints;

    [Header("Fallen warrior")]
    [SerializeField] private GameObject fallenWarriorPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float fallenWarriorX;
    [SerializeField] private float fallenWarriorY;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        checkpoints = FindObjectsOfType<Checkpoint>();
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData data)
    {
        LoadLostSoul(data);

        if (data == null || data.checkpoints == null)
            return;

        foreach (KeyValuePair<string, bool> kvp in data.checkpoints)
            checkpoints.FirstOrDefault(c => c.Id == kvp.Key).IsActivated = kvp.Value;

        if (ClosestActiveCheckpoint != null)
            PlayerManager.instance.player.transform.position =
                ClosestActiveCheckpoint.transform.position;
    }

    private void LoadLostSoul(GameData data)
    {
        lostCurrencyAmount = data.lostCurrency;
        fallenWarriorX = data.fallenWarriorX;
        fallenWarriorY = data.fallenWarriorY;

        if (lostCurrencyAmount > 0) // to not instantiate it if player just reloaded the game
        {
            GameObject newFallenWarrior = Instantiate(fallenWarriorPrefab, 
                new Vector3(fallenWarriorX, fallenWarriorY), Quaternion.identity);

            newFallenWarrior.GetComponent<FallenWarriorController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    public void SaveData(GameData data)
    {
        data.lostCurrency = lostCurrencyAmount;

        var player = PlayerManager.instance.player;

        data.fallenWarriorX = player.transform.position.x;
        data.fallenWarriorY = player.transform.position.y;

        data.checkpoints.Clear();

        foreach (var checkpoint in checkpoints)
            data.checkpoints.Add(checkpoint.Id, checkpoint.IsActivated);

        if (ClosestActiveCheckpoint != null)
            data.closestCheckpointID = ClosestActiveCheckpoint.Id;
    }

    private Checkpoint ClosestActiveCheckpoint => checkpoints
        .Where(c => c.IsActivated)
        .OrderBy(c => Vector2.Distance(
            PlayerManager.instance.player.transform.position,
            c.transform.position))
        .FirstOrDefault();
}
