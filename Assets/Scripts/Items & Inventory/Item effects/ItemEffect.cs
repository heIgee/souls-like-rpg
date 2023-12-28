using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Execute(Transform spawnTransform = null); 
    // do your magic 
}
