using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ResourceManager
{
    public static ResourceManager playerResources;

    public int Wood;
    public int Stone;
    public int Metal;
    public int Crystal;
    public int DarkMetal;

    public static ResourceManager operator -(ResourceManager a, ResourceManager b) => new ResourceManager(Mathf.RoundToInt(a.Wood - b.Wood), Mathf.RoundToInt(a.Stone - b.Stone), Mathf.RoundToInt(a.Metal - b.Metal), Mathf.RoundToInt(a.Crystal - b.Crystal), Mathf.RoundToInt(a.DarkMetal - b.DarkMetal));
    public static ResourceManager operator *(ResourceManager a, float b) => new ResourceManager(Mathf.RoundToInt(a.Wood * b), Mathf.RoundToInt(a.Stone * b), Mathf.RoundToInt(a.Metal * b), Mathf.RoundToInt(a.Crystal * b), Mathf.RoundToInt(a.DarkMetal * b));
    public static bool operator >(ResourceManager a, ResourceManager b) => ((a.Wood > b.Wood) && (a.Stone > b.Stone) && (a.Metal > b.Metal) && (a.Crystal > b.Crystal) && (a.DarkMetal > b.DarkMetal));
    public static bool operator <(ResourceManager a, ResourceManager b) => ((a.Wood < b.Wood) && (a.Stone < b.Stone) && (a.Metal < b.Metal) && (a.Crystal < b.Crystal) && (a.DarkMetal < b.DarkMetal));
    public static bool operator ==(ResourceManager a, ResourceManager b) => ((a.Wood == b.Wood) && (a.Stone == b.Stone) && (a.Metal == b.Metal) && (a.Crystal == b.Crystal) && (a.DarkMetal == b.DarkMetal));
    public static bool operator !=(ResourceManager a, ResourceManager b) => ((a.Wood != b.Wood) && (a.Stone != b.Stone) && (a.Metal != b.Metal) && (a.Crystal != b.Crystal) && (a.DarkMetal != b.DarkMetal));

    public event Action OnValueChange;

    public ResourceManager()
    {
        Wood = 0;
        Stone = 0;
        Metal = 0;
        Crystal = 0;
        DarkMetal = 0;
    }

    public ResourceManager(int wood, int stone, int metal, int space, int magic)
    {
        Wood = wood;
        Stone = stone;
        Metal = metal;
        Crystal = space;
        DarkMetal = magic;
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public bool CanAfford(ResourceManager cost)
    {
        return (Wood >= cost.Wood) && (Stone >= cost.Stone) && (Metal >= cost.Metal) && (Crystal >= cost.Crystal) && (DarkMetal >= cost.DarkMetal);
    }

    public void Add(ResourceManager add)
    {
        Wood += add.Wood;
        Stone += add.Stone;
        Metal += add.Metal;
        Crystal += add.Crystal;
        DarkMetal += add.DarkMetal;
        OnValueChange?.Invoke();
    }

    public void Subtract(ResourceManager subtract)
    {
        Wood -= subtract.Wood;
        Stone -= subtract.Stone;
        Metal -= subtract.Metal;
        Crystal -= subtract.Crystal;
        DarkMetal -= subtract.DarkMetal;
        OnValueChange?.Invoke();
    }

    public void Set(ResourceManager value)
    {
        Set(value.Wood, value.Stone, value.Metal, value.Crystal, value.DarkMetal);
    }

    public void Set(int wood, int stone, int metal, int crystal, int darkMetal)
    {
        Wood = wood;
        Stone = stone;
        Metal = metal;
        Crystal = crystal;
        DarkMetal = darkMetal;
        OnValueChange?.Invoke();
    }
}
