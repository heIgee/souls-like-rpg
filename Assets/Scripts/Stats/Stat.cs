using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat 
{
    [SerializeField] private int baseValue;

    public readonly List<int> modifiers = new();

    public int Value => baseValue + modifiers.Sum();

    public void SetBaseValue(int value) => baseValue = value;
    public void AddModifier(int modifier) => modifiers.Add(modifier); 
    public void RemoveModifier(int modifier) => modifiers.Remove(modifier); 
}
