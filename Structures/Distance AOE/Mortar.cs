using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mortar : Structure
{
    [SerializeField] protected float _explosionRadius;
    [SerializeField] SpriteAnimator frameAnimator;
    protected float explosionRadius { get { return (_explosionRadius + 0.5f) * MapGenerator.CellSize; } }
    protected Vector3 lastKnownPosition;

    [Header("Projectile")]
    [SerializeField] protected Projectile projectileGo;
    [SerializeField] AnimationCurve trajectory;
    [SerializeField] float spriteScaling = 1;
    [SerializeField] float _projectileSpeed;
    protected float projectileSpeed { get { return _projectileSpeed * MapGenerator.CellSize; } }
    protected Transform proj;
    protected delegate void OnProjectileHit(Vector3 point);
    protected OnProjectileHit OnProjHit;
    [Header("Explosion")]
    [SerializeField] protected GameObject _explosion;
    [SerializeField] protected SoundClip _explosionSound;


    protected override void Awake()
    {
        base.Awake();
        OnProjHit += SpawnExplosion;
    }

    private void FixedUpdate()
    {
        if (attackTarget != null)
        {
            lastKnownPosition = attackTarget.transform.position;
        }
    }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {

        if (frameAnimator.GetCurrentAnimationName() != "Idle")
        {
            Debug.LogWarning("Tried to attack but my animation hasn't finished! Consider slowing down my attack speed, or speeding up my animation", this);
            return;
        }

        base.Attack(projSpeed);

        frameAnimator.SetAnimation("Fire");
        frameAnimator.QueueAnimation("Reload");
        frameAnimator.QueueAnimation("Idle");

    }

    //Fire is called by the animator, doesn't need reference
    public virtual void Fire()
    {
        PlayFireSound();
        if (attackTarget == null) { return; }
        Projectile projectile = Projectile.CreateNewProjectile(projectileGo, transform.position, attackTarget.transform.position, projectileSpeed);
        proj = projectile.transform;
        StartCoroutine(ScaleShot(transform.position, attackTarget.transform.position, projectile));
        StartCoroutine(WaitForProjectile(projectile));
    }

    private IEnumerator ScaleShot(Vector2 origin, Vector2 destination, Projectile projectile)
    {
        float totalDistance = Vector2.Distance(origin, destination);
        float t;
        Vector3 startingScale = projectile.transform.localScale;
        Vector3 targetScale = projectile.transform.localScale * spriteScaling;
        while (projectile != null)
        {
            float travelledDistance = Vector2.Distance(projectile.transform.position, origin);
            t = Mathf.Clamp01(travelledDistance / totalDistance);
            projectile.transform.localScale = Vector3.Lerp(startingScale, targetScale, trajectory.Evaluate(t));
            yield return null;
        }
    }

    protected virtual IEnumerator WaitForProjectile(Projectile proj)
    {
        Vector2 projPos = new Vector2();
        while (proj != null)
        {
            projPos = proj.transform.position;
            yield return null;
        }

        Enemy[] enemies = GetEnemiesInRadius(explosionRadius, projPos);
        foreach (Enemy enemy in enemies)
        {
            bool died = enemy.TakeDamageInstant(structureSO.Damage);
            if (died)
                Kills++;
        }
        OnProjHit?.Invoke(projPos);
    }

    protected override float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    private void OnDrawGizmos()
    {
        if (proj != null)
        {
            Gizmos.DrawWireSphere(proj.transform.position, explosionRadius);
        }
    }

    protected void SpawnExplosion(Vector3 position)
    {
        if (_explosion == null)
            return;

        Transform explosionInstance = Instantiate(_explosion, position, Quaternion.identity).transform;
        PlayExplosionSound(explosionInstance);
    }

    protected virtual void PlayExplosionSound(Transform location)
    {
        SoundManager.PlaySound(_explosionSound, location);
    }
}
