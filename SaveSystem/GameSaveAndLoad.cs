using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class GameSaveAndLoad : MonoBehaviour
{
    static GameSaveAndLoad _instance;
    static string Filename { get => "gamesave.json"; }

    public static GameSave save;

    [SerializeField] StructureArraySO prefabsSO;
    Structure[] prefabs { get => prefabsSO.Structures; }

    private void Awake()
    {
        _instance = this;
    }

    public static void SaveGame()
    {
        _instance.Save();
    }

    [ContextMenu("Save")]
    public void Save()
    {
        save = new GameSave();
        int num = Mathf.CeilToInt(MapGenerator.HomeSize);
        for (int x = -num; x < num +1; x++)
        {
            for (int y = -num; y < num +1; y++)
            {
                TileObject tile = MapGenerator.Grid.GetGridObject(x + MapGenerator.Grid.CentreX , y + MapGenerator.Grid.CentreY);
                if (!tile.biome.IsHome || tile.WorldObject == null)
                    continue;

                save.structures.Add(((Structure)tile.WorldObject).name);
                save.pos.Add(new Vector2Int(x + MapGenerator.Grid.CentreX, y + MapGenerator.Grid.CentreY));
            }
        }
        save.resources = ResourceManager.playerResources;
        save.gold = GoldManager.PlayerGold.GetGold();
        save.day = GameManager.DayNumber;
        save.axeLevel = Player.GetAxeLevel();
        save.shoeLevel = CharacterController.GetShoeLevel();
        save.seed = MapGenerator.Seed;
        save.landUpgradeIndex = LandUpgrader.GetUpgradeIndex();
        save.score = ScoreManager.GetScore();
        save.resourcesScore = ScoreManager.GetResourceScore();
        save.killScore = ScoreManager.GetKillScore();
        save.WriteToFile(Filename);

        FloatingText.Create("GAME SAVED", Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)), Color.white, 2, 0.5f, 20);
    }

    public static void InitalLoad()
    {
        MapGenerator.Seed = save.seed;
    }

    public void CreateNewSave()
    {
        save = new GameSave();
    }

    public void LoadSave()
    {
        save = SaveableData.ReadFromFile<GameSave>(Filename);
    }

    public void LateLoad()
    {
        ResourceManager.playerResources.Set(save.resources);
        GoldManager.PlayerGold.SetGold(save.gold);
        GameManager.SetDay(save.day);
        Player.SetAxeLevel(save.axeLevel);
        CharacterController.SetShoeLevel(save.shoeLevel);
        LandUpgrader.SetUpgrade(save.landUpgradeIndex);
        MapGenerator.SetHomeSize(MapGenerator.HomeSize);
        ScoreManager.SetScore(save.score);
        ScoreManager.SetResource(save.resourcesScore);
        ScoreManager.SetKill(save.killScore);

        for (int i = 0; i < save.structures.Count; i++)
        {
            foreach (Structure structure in _instance.prefabs)
            {
                if (structure.name == save.structures[i])
                {
                    TileObject tile = MapGenerator.Grid.GetGridObject(save.pos[i].x, save.pos[i].y);
                    tile.CreateNewWorldObject(structure).tile = tile;
                }
            }
        }
    }

    [ContextMenu("Open")]
    public void Open()
    {
        Process.Start(Application.persistentDataPath);
    }

    public static GameSave TryGetSave()
    {
        return SaveableData.ReadFromFile<GameSave>(Filename);
    }

    public static void DeleteSave()
    {
        File.WriteAllText(Application.persistentDataPath + "/" + Filename, null);
    }
}

[System.Serializable]
public class GameSave : SaveableData
{
    public string Name;

    public List<string> structures;
    public List<Vector2Int> pos;
    public ResourceManager resources;
    public int gold = 0;
    public int day = 1;
    public int axeLevel = 0;
    public int shoeLevel = 0;
    public int seed;
    public int landUpgradeIndex = 0;
    public int score = 0;
    public int resourcesScore = 0;
    public int killScore = 0;


    public GameSave()
    {
        structures = new List<string>();
        pos = new List<Vector2Int>();
        resources = new ResourceManager(0, 0, 0, 0, 0);
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}