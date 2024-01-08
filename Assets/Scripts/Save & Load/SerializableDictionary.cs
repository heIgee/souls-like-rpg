using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    public void OnBeforeSerialize()
    {
        keys = new();
        values = new();

        foreach (KeyValuePair<TKey, TValue> kvp in this)
        {
            //Debug.Log($"{kvp.Key}:{kvp.Value}");
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        if (keys.Count <= 0)
        {
            //Debug.LogWarning("Serializable dictionary is fucking empty");
            return;
        }

        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys != values for some hellish reason");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
            Add(keys[i], values[i]);
    }
}
