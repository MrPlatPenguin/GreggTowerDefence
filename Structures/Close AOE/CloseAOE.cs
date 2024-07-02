using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class CloseAOE : Structure
{
    [SerializeField] protected Projectile[] projectilePrefabs;
    [SerializeField] protected float angle = 90;
    [SerializeField] protected float _projectileSpeed = 3;
    [SerializeField] protected int _projectileCount = 5;
    [SerializeField] protected float _projectileRandomness;
    [SerializeField] protected bool animOverride = false;
    float projectileSpeed { get { return _projectileSpeed * MapGenerator.CellSize; } }
    [SerializeField] protected Transform _projOrigin;

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        base.Attack(projSpeed);
        if (animOverride)
        {
            return;
        }

        animator.SetAnimation("Fire");
        animator.QueueAnimation("Reload");
        animator.QueueAnimation("Idle");

    }

    //Fire is called by the animator, doesn't need reference
    public virtual void Fire()
    {
        PlayFireSound();

        Vector3 originalVector = Quaternion.AngleAxis(angle * 0.5f, Vector3.forward) * rotatingPart.up;
        Vector3 rotatedVector;

        if (_projectileCount > 1)
        {
            for (int i = 0; i < _projectileCount; i++)
            {
                float randomness = Random.Range(-_projectileRandomness, _projectileRandomness);
                rotatedVector = Quaternion.AngleAxis(((-angle / (_projectileCount - 1)) * i) + randomness, Vector3.forward) * originalVector;
                Vector3 targetLocation = _projOrigin.position + (rotatedVector.normalized * structureSO.Range);

                Projectile prefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];

                Projectile.CreateNewProjectile(prefab, _projOrigin.position, targetLocation, _projectileSpeed);
            }
        }
        else if (_projectileCount == 1)
        {
            Projectile prefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];
            Vector2 spawnPostion = _projOrigin.position;
            Vector2 targetPosition = spawnPostion + (Vector2)(rotatingPart.up * structureSO.Range);
            Projectile.CreateNewProjectile(prefab, spawnPostion, targetPosition, _projectileSpeed);
        }

        Enemy[] enemies = GetEnemiesInRange();
        foreach (Enemy enemy in enemies)
        {
            if (PointInAngle(enemy.transform.position, transform.position, rotatingPart.up, angle))
            {
                float delay = Vector2.Distance(_projOrigin.position, enemy.transform.position) / _projectileSpeed;

                bool died = enemy.TakeDamageDelay(structureSO.Damage, delay);
                if (died)
                    Kills++;
            }
        }
    }

    protected override float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
}
