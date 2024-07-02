using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hwacha : RapidFire
{
    [SerializeField] Projectile _arrowGo;

    [SerializeField] GameObject smoke;
    //[SerializeField] AnimationCollection animColl;

    [SerializeField] SpriteAnimator gear;
    [SerializeField] SpriteAnimator topFrame;

    private bool rotating = false;

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        topFrame.SetAnimation("Recoil");
        topFrame.QueueAnimation("Idle Top Frame");

        SpriteAnimator chosenArrow = arrowAnimators[GetAllRandomIndex(arrowAnimators.Count)];
        chosenArrow.SetAnimation("Fire");
        chosenArrow.QueueAnimation("Idle Arrows");

        Projectile.CreateNewProjectile(_arrowGo, lastFiredArrow.position, attackTarget.transform.position, projectileSpeed);
        PlayFireSound();
        Instantiate(smoke, lastFiredArrow.position, lastFiredArrow.rotation);
        base.Attack(projSpeed);
    }

    protected override void Update()
    {
        base.Update();

        if (rotating)
        {
            rotating = false;
            return;
        }

        gear.QueueAnimation("Idle Gears");
    }

    protected override void RotateTurret(Enemy target)
    {
        base.RotateTurret(target);

        rotating = true;
        if (gear.GetCurrentAnimationName() != "Rotate")
        {
            gear.SetAnimation("Rotate");
        }
    }

    protected override float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
