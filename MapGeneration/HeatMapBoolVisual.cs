using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapBoolVisual : MonoBehaviour
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;
    Grid<bool> grid;
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

    public void SetGrid(Grid<bool> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    void Grid_OnGridValueChanged(object sender, Grid<bool>.OnGridValueChangedEventArgs e)
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


                bool gridValue = grid.GetGridObject(x, y);
                float gridValueNormalized = gridValue ? 1f : 0f;
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);                                                                     // vvvvvvv This shows the texture on the quad
                MeshUtils.AddToMeshArrays(verticies, uv, triangles, index, grid.GetWorldPosition(x, y) + (quadSize * 0.5f), 0f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}


