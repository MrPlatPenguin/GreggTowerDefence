using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float Duration = 1;
    float _destroyTime;

    private void Awake()
    {
        _destroyTime = Time.time + Duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _destroyTime)
            Destroy(gameObject);
    }
}
