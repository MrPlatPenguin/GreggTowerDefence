using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalExplosion : MonoBehaviour
{
    [SerializeField] float speed = 1;

    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 direction = (child.position - transform.position).normalized;
            child.position += child.localPosition.normalized * Time.deltaTime * speed;
            child.localScale -= Vector3.one * 1f * Time.deltaTime;
        }
    }
}
