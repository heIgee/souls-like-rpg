using System.Collections.Generic;
using UnityEditor.Overlays;

[System.Serializable]
public class GameData
{
    public int currency;

    public List<string> equipment = new();
    public List<string> inventory = new();
    public SerializableDictionary<string, int> stash = new();

    public SerializableDictionary<string, bool> skills = new();

    public SerializableDictionary<string, bool> checkpoints = new();
    public string closestCheckpointID = string.Empty;

    public float fallenWarriorX = 0;
    public float fallenWarriorY = 0;
    public int lostCurrency = 0;

    public GameData()
    {
    }
}
