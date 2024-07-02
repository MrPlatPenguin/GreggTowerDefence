using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;
    Grid<HeatMapGridObject> grid;
    Mesh mesh;
    bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    public void SetGrid(Grid<HeatMapGridObject> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    void Grid_OnGridValueChanged(object sender, Grid<HeatMapGridObject>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
    }

    void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] verticies, out Vector2[] uv, out int[] triangles);
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int index = x * grid.Height + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.CellSize;

                HeatMapGridObject gridValue = grid.GetGridObject(x, y);
                float gridValueNormalized = gridValue.GetValueNormalized();
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);                                                                     // vvvvvvv This shows the texture on the quad
                MeshUtils.AddToMeshArrays(verticies, uv, triangles, index, grid.GetWorldPosition(x, y) + (quadSize * 0.5f), 0f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}

public class HeatMapGridObject
{
    protected Grid<HeatMapGridObject> grid;
    private const int MIN = 0;
    private const int MAX = 100;
    protected int x;
    protected int y;
    int value;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int value)
    {
        this.value += value;
        this.value = Mathf.Clamp(this.value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public void OnClick()
    {
        AddValueInCircle(100, 5, true);
    }

    public override string ToString()
    {
        return GetValue().ToString();
    }

    public void AddValueInCircle(int value, int radius, bool falloff)
    {
        for (int x = -radius + 1; x < radius; x++)
        {
            for (int y = -radius + 1; y < radius; y++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
                float num = (1 - (distance / radius)) * value;
                if (distance <= radius - 0.5f)
                    grid.GetGridObject(this.x + x, this.y + y)?.AddValue(falloff ? (int)num : value);
            }
        }
    }

    public void AddValueInSquare(int value, int radius, bool falloff)
    {
        for (int x = -radius + 1; x < radius; x++)
        {
            for (int y = -radius + 1; y < radius; y++)
            {
                float distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                float num = (1 - (distance / radius)) * value;
                if (distance <= radius - 0.5f)
                    (grid.GetGridObject(this.x + x, this.y + y))?.AddValue(falloff ? (int)num : value);
            }
        }
    }

    public void AddValueInDiamond(int value, int radius, bool falloff)
    {
        for (int x = -radius + 1; x < radius; x++)
        {
            for (int y = -radius + 1; y < radius; y++)
            {
                float distance = Mathf.Abs(x) + Mathf.Abs(y);
                float num = (1 - (distance / radius)) * value;
                if (distance <= radius - 0.5f)
                    (grid.GetGridObject(this.x + x, this.y + y))?.AddValue(falloff ? (int)num : value);
            }
        }
    }

    public int GetValue()
    {
        return value;
    }
}
