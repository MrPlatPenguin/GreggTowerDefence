using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearPoker : RapidFire
{

    [SerializeField] GameObject poof;

    private bool rotating = false;
    [SerializeField] Transform poofSpawnOrigin;

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        SpriteAnimator chosenArrow = arrowAnimators[GetAllRandomIndex(arrowAnimators.Count)];
        chosenArrow.SetAnimation("Fire");
        chosenArrow.QueueAnimation("Idle");

        base.Attack(projSpeed);

        Instantiate(poof, poofSpawnOrigin.position, poofSpawnOrigin.rotation);

        PlayFireSound();
    }

    protected override void Update()
    {
        base.Update();

        if (rotating)
        {
            rotating = false;
            return;
        }
    }

    protected override float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
