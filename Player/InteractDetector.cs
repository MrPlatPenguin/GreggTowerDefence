using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractDetector : MonoBehaviour
{
    static InteractDetector instance;

    Vector2Int prevMouseCellLocation;
    TileObject CurrentTileObject;

    [SerializeField] Vector2Int currentMouseCellLocation;
    private void Awake()
    {
        instance= this;
    }

    private void Update()
    {
        currentMouseCellLocation = GetMouseCell();

        // Checks if clicking in the map
        if (currentMouseCellLocation.x < 0 || currentMouseCellLocation.y < 0 || currentMouseCellLocation.x > 167 || currentMouseCellLocation.y > 167)
            return;

        CurrentTileObject = MapGenerator.Grid.GetGridObject(currentMouseCellLocation.x, currentMouseCellLocation.y);
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetButtonDown("Interact"))
            MouseDown(currentMouseCellLocation);
        else if (Input.GetButtonUp("Interact"))
            MouseUp();
    }

    Vector2Int GetMouseCell()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MapGenerator.Grid.GetXY(mousePos, out int x, out int y);
        return new Vector2Int(x, y);
    }

    private void MouseDown(Vector2Int currentMouseCellLocation)
    {
        if (currentMouseCellLocation != prevMouseCellLocation && MapGenerator.Grid.GetGridObject(prevMouseCellLocation.x, prevMouseCellLocation.y).WorldObject is Structure)
        {
            Structure s = (Structure)(MapGenerator.Grid.GetGridObject(prevMouseCellLocation.x, prevMouseCellLocation.y).WorldObject);
            s.Deselect();
        }

        CurrentTileObject.WorldObject?.OnClickStart(CurrentTileObject);

        if (Builder.BuildMode == BuildMode.Build)
        {
            if (CurrentTileObject.biome.Name == "Home" && CurrentTileObject.WorldObject == null)
                Builder.Instance.Build(CurrentTileObject);
            else if (CurrentTileObject.biome.Name != "Home")
                FloatingText.Create("Invalid Position", MapGenerator.Grid.GetGridCentreWorldPosition(currentMouseCellLocation.x, currentMouseCellLocation.y), Color.red);
            else if (CurrentTileObject.WorldObject != null)
                FloatingText.Create("Structure Obstructed", MapGenerator.Grid.GetGridCentreWorldPosition(currentMouseCellLocation.x, currentMouseCellLocation.y), Color.red);
        }
        prevMouseCellLocation = currentMouseCellLocation;
    }

    private void MouseUp()
    {
        if (CurrentTileObject.WorldObject != null)
        {
            CurrentTileObject.WorldObject.OnClickEnd(CurrentTileObject);
        }
    }

    public static WorldObject MouseOverWorldObject()
    {
        bool isNull = instance.CurrentTileObject == null || instance.CurrentTileObject.WorldObject == null;
        if (isNull)
            return null;

        return instance.CurrentTileObject.WorldObject;
    }
}
