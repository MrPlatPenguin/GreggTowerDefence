using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab;
    public EnemySO[] enemies;
    private Coroutine SpawnCo;

    int enemyIndex = 0;

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < 500; i++)
            {
                Vector2 pos = (Random.insideUnitCircle.normalized * MapGenerator.CellSize * 3) + MapGenerator.MapCentre;
                SpawnEnemy(0, pos);
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            SpawnCo = StartCoroutine(SpawnLoads());
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousepos.z = 0;
            SpawnEnemy(enemyIndex, mousepos);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            StopCoroutine(SpawnCo);
        }

        if (Input.mouseScrollDelta.y < 0)
            enemyIndex++;
        else if (Input.mouseScrollDelta.y > 0)
            enemyIndex--;

        enemyIndex = Mathf.Clamp(enemyIndex, 0, enemies.Length - 1);

        //if (enemyIndex < 0)
        //    enemyIndex += enemies.Length;
        if (enemyIndex < 0)
            enemyIndex += enemies.Length;
        if (enemyIndex < 0)
            enemyIndex += enemies.Length;
    }

    void SpawnEnemy(int index, Vector2 pos)
    {
        enemyPrefab.SpawnEnemy(enemies[index], pos);
    }

    void SpawnEnemy()
    {
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousepos.z = 0;
        SpawnEnemy(enemyIndex, mousepos);
    }

    private IEnumerator SpawnLoads()
    {
        yield return new WaitForSeconds(0.5f);
        while (Input.GetMouseButton(2))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousepos.z = 0;
            SpawnEnemy(enemyIndex, mousepos);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
