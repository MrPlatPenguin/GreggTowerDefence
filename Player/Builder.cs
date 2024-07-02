using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildMode
{
    None,
    Build,
    Upgrade,
    Destroy,
    BuildMenu
}

public class Builder : MonoBehaviour
{
    [field:SerializeField] public StructureArraySO[] StructureTiers { get; private set; }
    public static Builder Instance { get; private set; }

    [SerializeField] ResourceManager resources;

    [SerializeField] SpriteRenderer _hologramDisplay;

    public static bool IsBuilding { get => BuildMode == BuildMode.Build; }
    Structure _buildingStructure;

    BuildMode _buildMode = BuildMode.None;
    public static BuildMode BuildMode { get => Instance._buildMode; }

    public delegate void onBuildModeChange(BuildMode newBuildMode);
    onBuildModeChange _onBuildModeChange;
    public static onBuildModeChange OnBuildModeChange { get => Instance._onBuildModeChange; set => Instance._onBuildModeChange = value; }

    event Action _onNextBuild;
    public static event Action OnNextBuild { add => Instance._onNextBuild += value; remove => Instance._onNextBuild -= value; }

    event Action _upgradeHint;
    public static event Action UpgradeHint { add => Instance._upgradeHint += value; remove => Instance._upgradeHint -= value; }

    private void Awake()
    {
        Instance = this;
        ResourceManager.playerResources = resources;
        Structure.hasUpgraded = false;
    }

    private void Update()
    {
        if (IsBuilding)
        {
            int mouseX, mouseY;
            MapGenerator.Grid.GetXY(Camera.main.ScreenToWorldPoint(Input.mousePosition), out mouseX, out mouseY);
            _hologramDisplay.transform.position = MapGenerator.Grid.GetGridCentreWorldPosition(mouseX, mouseY);
            ResourceNumbers.ShowCost(_buildingStructure.structureSO.Cost);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SideMenuUI.OpenBuildMenu();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetBuildMode(BuildMode.Upgrade);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetBuildMode(BuildMode.Destroy);
        if (Input.GetKeyDown(KeyCode.R))
            SetBuildMode(BuildMode.None);
    }

    private void LateUpdate()
    {
        if (Input.GetButtonDown("CancelBuilding"))
        {
            StopBuilding();
            SideMenuUI.CloseBuildMenu();
        }
    }

    public static BuildMode GetBuildMode()
    {
        return Instance._buildMode;
    }

    public Structure Build(TileObject tileObject) 
    {
        if (IsBuilding)
        {
            _onNextBuild?.Invoke();
            _onNextBuild = null;

            Structure structure = tileObject.CreateNewWorldObject(_buildingStructure);
            structure.tile = tileObject;
            if (structure.structureSO.Tier > StructureMaterial.Wood && !Structure.hasUpgraded)
                _upgradeHint?.Invoke();

            ResourceManager.playerResources.Subtract(_buildingStructure.structureSO.Cost);
            if (!ResourceManager.playerResources.CanAfford(_buildingStructure.structureSO.Cost))
                StopBuilding();
            return structure;
        }

        return null;
    }

    public void StopBuilding()
    {
        SetBuildMode(BuildMode.None);

        _hologramDisplay.gameObject.SetActive(false);
        _buildingStructure = null;
        ResourceNumbers.HideCost();
    }

    public Structure GetStructure(int tier, int index)
    {
        if (tier > StructureTiers.Length - 1 || index > StructureTiers[tier].Structures.Length - 1)
            return null;
        return StructureTiers[tier].Structures[index];
    }

    public Structure[] GetStructuresInTier(int tier)
    {
        if (tier > StructureTiers.Length -1)
            return null;
        return StructureTiers[tier].Structures;
    }

    public static void SetBuildMode(BuildMode buildMode)
    {
        BuildMode oldBuildMode = Instance._buildMode;
        Instance._buildMode = buildMode;

        if (buildMode == BuildMode.None)
            GameManager.UnpauseGame();
        else if (oldBuildMode == BuildMode.None)
            GameManager.PauseGame();

        OnBuildModeChange?.Invoke(buildMode);
    }

    public static void SetBuildMode(Structure structure)
    {
        SetBuildMode(BuildMode.Build);
        Instance._hologramDisplay.sprite = structure.structureSO.Thumbnail;
        Instance._buildingStructure = structure;
        Instance._hologramDisplay.gameObject.SetActive(true);

        Instance._hologramDisplay.gameObject.DrawCircle(Instance._buildingStructure.structureSO.Range, 50, "Structure Range Circle");
    }
}

[System.Serializable]
public class StructureTier
{
    [SerializeField] string name;
    public Structure[] Structures;
}