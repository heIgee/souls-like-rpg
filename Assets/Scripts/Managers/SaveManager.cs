using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private GameData gameData;

    private List<ISaveManager> saveManagers;
    private FileDataHandler fileDataHandler;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    public bool HasSaveData => fileDataHandler.Load() != null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        saveManagers = FindAllSaveManagers();
        //Debug.LogError(saveManagers.Count);

        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
                                      
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.Load();

        if (gameData == null)
        {
            Debug.LogWarning("No save data found");
            NewGame();
        }

        foreach (var manager in saveManagers)
        {
            //Debug.Log($"Save manager: {manager}");
            manager.LoadData(gameData);
        }

        Debug.Log("Data loaded from save file");
    }

    public void SaveGame()
    {
        foreach (var manager in saveManagers)
        {
            //Debug.Log($"Manager {manager} saving data...");
            manager.SaveData(gameData);
        }

        fileDataHandler.Save(gameData);

        Debug.Log("Game saved");
    }

    [ContextMenu("Delete save file")]
    public void DeleteData()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        fileDataHandler.Delete();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
        => new(FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<ISaveManager>());
    // FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>() as List<ISaveManager> not working btw



}