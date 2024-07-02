using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Tesla : RapidFire
{
    [SerializeField] VisualEffect lightning;

    protected override void Awake()
    {
        lightning.Stop();
        base.Awake();
        lightning.SetFloat("Delay", structureSO.TimeBetweenAttacks);
        lightning.SetVector3("Start", transform.position);
        lightning.enabled = false;
    }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        lightning.enabled = true;
        lightning.SetVector3("End", attackTarget.transform.position);
        lightning.Play();
        base.Attack(projSpeed);
    }

    protected override float GetProjectileSpeed()
    {
        return Mathf.Infinity;
    }
}
