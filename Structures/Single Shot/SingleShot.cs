using System.Collections.Generic;
using UnityEngine;


public abstract class SingleShot : Structure
{
    [SerializeField] protected int maxTargets = 1;

    [SerializeField] float _projectileSpeed = 20f;
    [SerializeField] Projectile _projectileGo;
    protected float projectileSpeed { get { return _projectileSpeed * MapGenerator.CellSize; } }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        base.Attack(projSpeed);
        AnimationQueue();
        PlayFireSound();
    }

    protected override float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    protected virtual void GetRemainingTargets(out List<Enemy> targets, out List<Transform> targetTransforms)
    {
        Vector2 direction = attackTarget.transform.position - transform.position;

        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, direction, Mathf.Infinity, enemyLayer);

        targets = new List<Enemy>();
        targetTransforms = new List<Transform>();

        int numEnemiesHit = 0;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            hitColliders[i].collider.gameObject.TryGetComponent(out Enemy enemy);

            if (targets.Contains(enemy))
                continue;

            targets.Add(enemy);
            targetTransforms.Add(enemy.transform);

            if (numEnemiesHit >= maxTargets)
                break;
        }
    }

    //Fire is called by the animator, doesn't need reference
    public void Fire()
    {
        if (attackTarget == null) return;

        List<Enemy> targets;
        List<Transform> targetTransforms;

        GetRemainingTargets(out targets, out targetTransforms);

        Vector2 previousEnemy = transform.position;

        float totalDelay = 0;

        for (int i = 0; i < Mathf.Min(targets.Count, maxTargets); i++)
        {
            Enemy target = targets[i];

            if (target == null)
                continue; 

            float delay = Vector2.Distance(previousEnemy, target.transform.position) / projectileSpeed;


            delay += totalDelay;
            totalDelay += delay - totalDelay;

            previousEnemy = target.transform.position;

            bool died = target.TakeDamageDelay(structureSO.Damage, delay);
            if (died)
                Kills++;
        }

        LaunchProjectile(targetTransforms);
    }

    protected virtual void LaunchProjectile(List<Transform> targets)
    {
        if (targets.Count <= 0)
            return;
        Projectile.CreateNewProjectile(_projectileGo, transform.position, targets[targets.Count - 1].position, projectileSpeed, targets.Count >= maxTargets);
    }
}