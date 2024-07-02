using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    static BoidManager instance;

    List<Boids> boids = new List<Boids>();

    List<Boids> boidsInRangeOfPlayer = new List<Boids>();
    float range;

    private void Awake()
    {
        range = 3 * MapGenerator.CellSize;
    }

    public static void AddBoid(Boids boid)
    {
        instance.boids.Add(boid);
    }

    public static void RemoveBoid(Boids boid)
    {
        instance.boids.Remove(boid);
    }

    private void FixedUpdate()
    {
        if (Player.InBase)
            return;

        if (Enemy.GetEnemyQuadTree() == null)
            return;

        foreach (Enemy enemy in Enemy.GetEnemyQuadTree().EnemiesInRadius(transform.position, range))
        {
            enemy.BoidAI.SetTargetingPlayer(true);
        }
    }
}
