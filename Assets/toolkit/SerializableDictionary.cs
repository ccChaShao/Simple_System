using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new ();
    [SerializeField] private List<TValue> values = new ();
        
    private Dictionary<TKey, TValue> targetDictionary = new ();

    public Dictionary<TKey, TValue> ToDictionary()
    {
        return targetDictionary;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dictionary)
    {
        targetDictionary = dictionary;
    }
        
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kv in targetDictionary)
        {
            keys.Add(kv.Key);
            values.Add(kv.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        targetDictionary.Clear();

        int minCount = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < minCount - 1; i++)
        {
            targetDictionary[keys[i]] = values[i];
        }
    }
}