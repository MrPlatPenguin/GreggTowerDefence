using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OrbitalCannon : Mortar
{

    //I know this is gross but fuck it.
    [SerializeField] float delayToDamage;

    private Enemy[] enemies;

    protected override void Awake()
    {
        base.Awake();
        OnProjHit += Explosion;
    }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        base.Attack(projSpeed);
        SoundManager.PlaySound(_fireSounds[0], transform);
    }

    public override void Fire()
    {
        if (attackTarget != null)
        {
            enemies = GetEnemiesInRadius(explosionRadius, attackTarget.transform.position);
            OnProjHit?.Invoke(attackTarget.transform.position);
            return;
        }

        enemies = GetEnemiesInRadius(explosionRadius, lastKnownPosition);
        OnProjHit?.Invoke(lastKnownPosition);
    }

    private void Explosion(Vector3 point)
    {
        foreach (Enemy enemy in enemies)
        {
            bool died = enemy.TakeDamageDelay(structureSO.Damage, delayToDamage);
            if (died)
                Kills++;
        }
    }
    protected override void PlayExplosionSound(Transform location)
    {
        new Delay(this, delayToDamage, () => base.PlayExplosionSound(location));
    }
}
