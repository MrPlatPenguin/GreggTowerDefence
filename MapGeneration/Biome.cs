using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Biome
{
    public string Name;
    [field:SerializeField] public float Size { get; private set; }
    public int TextureIndexY;
    public int TextureIndexX;
    [Range(0f, 1f)]
    [SerializeField] float chanceForResource;
    public ResourceWeight[] resources;

    public bool IsHome { get => Name == "Home"; }


    public Biome(string name, int textureIndexX, int textureIndexY)
    {
        Name = name;
        TextureIndexX = textureIndexX;
        TextureIndexY = textureIndexY;
    }

    public ResourceSO ChooseResource()
    {
        if (UnityEngine.Random.Range(0, (float)1) > chanceForResource)
            return null;

        float totalChance = 0;
        for (int i = 0; i < resources.Length; i++)
        {
            totalChance += resources[i].weight;
        }
        
        float rand = UnityEngine.Random.Range(0, totalChance);

        foreach (ResourceWeight resourceWeight in resources)
        {
            if (rand < resourceWeight.weight)
                return resourceWeight.resource;
            rand -= resourceWeight.weight;
        }
        return null;
    }
}

[Serializable] 
public class ResourceWeight
{
    public ResourceSO resource;
    public float weight;
}