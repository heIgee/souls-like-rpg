using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public string description;
    public abstract void Execute(Transform spawnTransform = null); 
    // do your magic 
}
