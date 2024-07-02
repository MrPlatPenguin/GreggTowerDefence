using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineGun : SingleShot
{
    //[SerializeField] float _projectileSpeed = 20f;
    //[SerializeField] GameObject _projectileGo;
    //float projectileSpeed { get { return _projectileSpeed * MapGenerator.CellSize; } }
    //[SerializeField] SpriteAnimator animator;

    //private Enemy[] targets;

    //bool hasTarget;

    //[SerializeField] Transform projSpawnOrigin;

    //protected override void Attack(float projSpeed = float.PositiveInfinity)
    //{
    //    base.Attack(projSpeed);
    //}

    //protected override Enemy TryGetAttackTarget()
    //{
    //    Enemy target = base.TryGetAttackTarget();

    //    if (hasTarget && target == null)
    //    {
    //        animator.SetAnimation("Idle");
    //        hasTarget = false;
    //    }

    //    else if (!hasTarget && target != null)
    //    {
    //        animator.SetAnimation("Fire");
    //        hasTarget = true;
    //    }
    //    return target;
    //}

    //protected override Transform GetTowerTransform()
    //{
    //    return transform;
    //}

    //protected override float GetProjectileSpeed()
    //{
    //    return projectileSpeed;
    //}

    ////Fire is called by the animator, doesn't need reference
    //public void Fire()
    //{
    //    if (targets == null || targets.Length == 0) return;

    //    Transform[] targetTransforms = new Transform[targets.Length];

    //    for (int i = 0; i < targets.Length; i++)
    //    {
    //        targetTransforms[i] = targets[i].transform;
    //    }

    //    Projectile.CreateNewProjectile(_projectileGo, projSpawnOrigin.position, targetTransforms, new float[] { 0 }, projectileSpeed, targets.Length < maxTargets ? false : true);
    //    SoundManager.PlaySound(_fireSound, SoundManager.SoundType.SFX, transform);

    //    Transform previousEnemy = transform;

    //    float totalDelay = 0;

    //    for (int i = 0; i < targets.Length; i++)
    //    {
    //        if (targets[i] != null)
    //        {
    //            float delay = Vector2.Distance(previousEnemy.position, targets[i].transform.position) / projectileSpeed;

    //            delay += totalDelay;
    //            totalDelay += delay - totalDelay;

    //            previousEnemy = targets[i].transform;

    //            Enemy target = targets[i];

    //            new Delay(target, delay, delegate
    //            {
    //                //print(target[i].name);
    //                target.TakeDamage(structureSO.Damage, delegate { Kills++; });
    //            });
    //        }
    //    }
    //}
}
