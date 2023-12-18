using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat 
{

    // serialize or public set

    [SerializeField] private int baseValue;

    public List<int> modifiers;

    // I'm cumming
    public int Value => baseValue + modifiers.Sum();

    public void SetDefault(int value) => baseValue = value;
    public void AddModifier(int modifier) => modifiers.Add(modifier); 
    public void RemoveModifier(int modifier) => modifiers.Remove(modifier); 

}
