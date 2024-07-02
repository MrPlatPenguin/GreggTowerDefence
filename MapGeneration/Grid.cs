using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid<TGridObject>
{
    public int ChunkSize;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Radius { get; private set; }
    public int CentreX { get; private set; }
    public int CentreY { get; private set; }
    TGridObject[,] gridArray;
    public float CellSize { get; private set; }
    Vector3 originPosition;
    private TextMesh[,] debugTextArray;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public Grid(int chunkSize, int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int , int, TGridObject> createGridObject)
    {
        this.ChunkSize = chunkSize;
        Radius = width / 2;
        width = width + (chunkSize - (width % ChunkSize));
        height = height + (chunkSize - (height % ChunkSize));

        this.Width = width;
        this.Height = height;
        CentreX = width / 2;
        CentreY = height / 2;
        this.CellSize = cellSize;
        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];
        this.originPosition = originPosition;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = false;
        if (showDebug)
            DrawDebug();

    }

    private void Grid_OnGridValueChanged(object sender, OnGridValueChangedEventArgs gridCell)
    {
        UpdateDebugText(gridCell.x, gridCell.y);
    }

    bool IsValidCell(int x, int y)
    {
        // Checks if cells are within bounds of grid
        return (x >= 0 && y >= 0 && x < Width && y < Height);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * CellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / CellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / CellSize);
    }

    public void GetChunkXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / (CellSize * ChunkSize));
        y = Mathf.FloorToInt((worldPosition - originPosition).y / (CellSize * ChunkSize));
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (!IsValidCell(x, y))
            return;
        gridArray[x, y] = value;
        if (OnGridValueChanged != null)
            OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridValueChanged != null)
            OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
       if (!IsValidCell(x, y))
            return default(TGridObject);

        return gridArray[x, y];
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public Vector2Int[] GetCellsInSquare(int originX, int originY, int range)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int x = -range + 1; x < range; x++)
        {
            for (int y = -range + 1; y < range; y++)
            {
                cells.Add(new Vector2Int(originX + x, originY + y));
            }
        }
        return cells.ToArray();
    }

    void UpdateDebugText(int x, int y)
    {
        debugTextArray[x, y].text = gridArray[x, y]?.ToString();
    }

    public void DrawDebug()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                //debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(CellSize, CellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, Height), GetWorldPosition(Width, Height), Color.white);
        Debug.DrawLine(GetWorldPosition(Width, 0), GetWorldPosition(Width, Height), Color.white);
        OnGridValueChanged += Grid_OnGridValueChanged;
    }

    public Vector2 GetGridCentreWorldPosition(int x, int y)
    {
        return GetWorldPosition(x, y) + new Vector3(CellSize * 0.5f, CellSize * 0.5f);
    }
}