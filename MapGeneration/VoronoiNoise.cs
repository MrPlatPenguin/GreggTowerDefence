using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class VoronoiNoise
{
    // Declare private variables
    private Vector2[,] points;
    private int[,] values;
    private float gridSize;

    // Constructor for VoronoiNoise class
    public VoronoiNoise(int width, int height, int frequency, float gridSize, int seed)
    {
        // Set gridSize based on frequency
        gridSize = gridSize * frequency;
        // Store variables
        this.gridSize = gridSize;

        // Divide width and height by frequency
        width = width / frequency;
        height = height / frequency;

        // Create points and values arrays
        points = new Vector2[width, height];
        values = new int[width, height];

        Random.InitState(seed);
        // Loop through all points and assign a random position and value
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Generate a random position within the grid cell
                points[x, y] = new Vector2(x + Random.Range(0f, 1f), y + (Random.Range(1f, 2f)) - 1f) * gridSize;


                // Assign a value to the point based on its position
                values[x, y] = SetValue(x, y);
            }
        }
        int i = 0;
    }

    // Convert world position to grid coordinates
    public void WorldToGridCoordinate(Vector2 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPos.x / gridSize);
        y = Mathf.FloorToInt(worldPos.y / gridSize);
    }

    // Get value for a world position
    public int GetValue(Vector2 worldPos)
    {
        // Convert world position to grid coordinates
        WorldToGridCoordinate(worldPos, out int xPoint, out int yPoint);

        // Find the closest point to the world position
        float closestPointDist = float.MaxValue;
        int closestX = 0;
        int closestY = 0;

        for (int x = xPoint - 1; x <= xPoint + 1; x++)
        {
            // Skip if out of bounds
            if (x < 0 || x > points.GetLength(0) - 1)
                continue;

            for (int y = yPoint - 1; y <= yPoint + 1; y++)
            {
                // Skip if out of bounds
                if (y < 0 || y > points.GetLength(1) - 1)
                    continue;

                // Calculate distance to point with some random variation
                float pointDistance = Vector2.Distance(worldPos, points[x, y] + Random.insideUnitCircle);
                if (pointDistance < closestPointDist)
                {
                    // Store the closest point
                    closestPointDist = pointDistance;
                    closestX = x;
                    closestY = y;
                }
            }
        }
        // Return the value of the closest point
        return values[closestX, closestY];
    }

    // Set value for a grid position
    int SetValue(int x, int y)
    {
        // Calculate distance from the centre of the grid
        float distanceFromCentre = (new Vector2(x, y) - new Vector2(points.GetLength(0) * 0.5f, points.GetLength(1) * 0.5f)).magnitude - 0.5f;
        // Calculate radius of the grid
        float radius = (points.GetLength(0) * 0.5f);

        float a = distanceFromCentre / radius;

        int desiredBiome = 6;
        for (int i = 0; i < MapGenerator.Biomes.Length; i++)
        {
            if (a < MapGenerator.Biomes[i].Size)
            {
                desiredBiome = i;
                break;
            }
        }

        return Mathf.Clamp(Random.Range(desiredBiome - 1, desiredBiome + 1), 1, MapGenerator.Biomes.Length - 2);
    }
}
