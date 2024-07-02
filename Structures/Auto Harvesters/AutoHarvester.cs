using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHarvester : Structure
{
    [SerializeField] ResourceManager _resourcesPerDay;
    [SerializeField] ItemResource item;
    int tier = 1;
    float timeSinceLastHarvest;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override float GetProjectileSpeed()
    {
        throw new System.NotImplementedException();
    }

    protected override Enemy TryGetAttackTarget()
    {
        return null;
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.IsNightTime)
            return;

        timeSinceLastHarvest += Time.deltaTime;

        if (timeSinceLastHarvest >= structureSO.TimeBetweenAttacks)
            Harvest();
    }

    void Harvest()
    {
        PlayFireSound();
        SpawnResource(item, true);
        timeSinceLastHarvest = 0;
        FloatingText.Create("+", transform.position, Color.white);
    }
}
