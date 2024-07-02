using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaler : MonoBehaviour
{
    [SerializeField] float scale = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        Rescale();
    }

    public void Rescale()
    {
        float scale = MapGenerator.CellSize * 2.5f * this.scale;
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
