using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SniperProjHack : MonoBehaviour
{
    [SerializeField]    VisualEffect sniperVFX;
    private void Update()
    {
        sniperVFX.SetVector3("Position", transform.position);
    }
}
