using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_Catapult_Launcher : MonoBehaviour
{
    Transform[] splinters;
    [SerializeField] float spread = 1;
    [SerializeField] float spreadSpeed = 1;
    [SerializeField] float spinSpeed = 1;
    int[] rotations;
    float prevScale;

    private void Awake()
    {
        splinters = new Transform[transform.childCount];
        rotations = new int[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle;
            if (i > transform.childCount * 0.5f)
                spawnPos.Normalize();
            else
                spawnPos = spawnPos * 0.5f;

            transform.GetChild(i).localPosition = spawnPos / transform.localScale.x * spread;
            splinters[i] = transform.GetChild(i);
            rotations[i] = Random.Range(-1, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (prevScale > transform.localScale.magnitude)
            {
                Vector3 direction = splinters[i].localPosition.normalized;
                direction.Normalize();

                splinters[i].localPosition += direction * spreadSpeed * Time.deltaTime;
            }
            splinters[i].transform.Rotate(transform.forward, Time.deltaTime * spinSpeed * rotations[i]);
        }
        prevScale = transform.localScale.magnitude;
    }
}