using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class QuadTree
{
    int capacity;
    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    Rect bounds;

    bool subdivided;
    [SerializeField]
    QuadTree[] children;
    QuadTree parent;

    public QuadTree(Rect bounds, int capacity, QuadTree parent = null)
    {
        this.bounds = bounds;
        this.capacity = capacity;
        this.parent = parent;
        children = new QuadTree[0];
    }

    void Subdivide()
    {
        children = new QuadTree[4];
        subdivided = true;

        // North West
        children[0] = new QuadTree(new Rect(bounds.x, bounds.y + (bounds.height * 0.5f), bounds.width / 2, bounds.height / 2), capacity, this);

        // North East
        children[1] = new QuadTree(new Rect(bounds.x + (bounds.width * 0.5f), bounds.y + (bounds.height * 0.5f), bounds.width / 2, bounds.height / 2), capacity, this);

        // South West
        children[2] = new QuadTree(new Rect(bounds.x, bounds.y, bounds.width / 2, bounds.height / 2), capacity, this);

        // South East
        children[3] = new QuadTree(new Rect(bounds.x + (bounds.width * 0.5f), bounds.y, bounds.width / 2, bounds.height / 2), capacity, this);

    }

    public QuadTree Insert(Enemy enemy)
    {
        if (!bounds.Contains(enemy.transform.position))
            return null;

        if (enemies.Count < capacity)
        {
            enemies.Add(enemy);
            return this;
        }
        else
        {
            if (!subdivided)
                Subdivide();
        }

        foreach (QuadTree child in children)
        {
            QuadTree qt = child.Insert(enemy);
            if (qt != null)
                return qt;
        }
        return null;
    }

    public bool Remove(Enemy enemy)
    {
        if (!bounds.Contains(enemy.transform.position))
            return false;

        foreach (Enemy checkEnemy in enemies)
        {
            if (checkEnemy == enemy)
            {
                enemies.Remove(checkEnemy);
                //if (parent != null)
                //    parent.Merge();
                return true;
            }
        }

        foreach (QuadTree child in children)
        {
            if (child.Remove(enemy))
                break;
        }
        return false;
    }

    public void RemoveDirty(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    void Merge()
    {
        foreach (QuadTree child in children)
        {
            if (child.enemies.Count > 0)
                return;
        }
        children = new QuadTree[0];
        subdivided = false;
    }

    public QuadTree Update(Enemy enemy)
    {
        Remove(enemy);
        return Insert(enemy);
    }

    public QuadTree UpdateDirty(Enemy enemy, QuadTree exactQuadTree)
    {
        if (exactQuadTree.bounds.Contains(enemy.transform.position))
            return exactQuadTree;

        exactQuadTree.RemoveDirty(enemy);
        return Insert(enemy);
    }

    public Enemy[] EnemiesInRect(Rect range)
    {
        if (!range.Overlaps(bounds))
            return new Enemy[0];

        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (range.Contains(enemy.transform.position))
            {
                enemiesInRange.Add(enemy);
            }
        }

        foreach (QuadTree child in children)
        {
            enemiesInRange.Concat(child.EnemiesInRect(range));
        }

        return enemiesInRange.ToArray();
    }

    public Enemy[] EnemiesInRadius(Vector2 origin, float radius)
    {
        if (radius <= 0)
            return new Enemy[0];

        Rect range = new Rect(origin.x - radius, origin.y - radius, radius * 2, radius * 2);
        //DrawRect(range);
        if (!range.Overlaps(bounds))
            return new Enemy[0];

        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (Vector2.Distance(origin, enemy.transform.position) <= radius)
            {
                enemiesInRange.Add(enemy);
            }
        }

        foreach (QuadTree child in children)
        {
            enemiesInRange.AddRange(child.EnemiesInRadius(origin, radius));
        }

        return enemiesInRange.ToArray();
    }

    public void DrawQuad(Color color, bool single = false)
    {
        Rect rect = bounds;
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), color);
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), color);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), color);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), color);

        if (children == null || single)
            return;

        for (int i = 0; i < children.Length; i++)
        {
            children[i].DrawQuad(color);
        }
    }

    void DrawRect(Rect rect)
    {
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.white);
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.white);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.white);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.white);
    }
}
