using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.VFX;

public class PlasmaChainer : CloseAOE
{
    [SerializeField] Transform proj2Origin;
    [SerializeField] Transform endPointA;
    [SerializeField] Transform endPointB;
    [SerializeField] float _extensionDuration;
    [SerializeField] float tickDuration;
    [SerializeField] VisualEffect lightning;
    [SerializeField] float retractionTime = 0.5f;
    bool isTargeting;

    VisualEffect VEA;
    VisualEffect VEB;
    List<Transform> projTransfroms;

    private bool chainsOut;

    protected override void Awake()
    {
        base.Awake();
        VEA = _projOrigin.GetComponent<VisualEffect>();
        VEB = proj2Origin.GetComponent<VisualEffect>();
        VEA.enabled = false;
        VEB.enabled = false;
    }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        if (chainsOut)
        {
            //print("Hey! I'm  a chainlauncher and I tried to fire my chains but they're already out! This is probably because my time between attacks is too short, or my extention duration is too long. FIX IT!");
            timeSinceLastAttack = 1000;
            return;
        }
        base.Attack(projSpeed);
        animator.SetAnimation("Fire");
        animator.QueueAnimation("Idle Unloaded");
    }

    protected override Enemy TryGetAttackTarget()
    {
        if (isTargeting)
            return null;
        else
            return base.TryGetAttackTarget();
    }

    public override void Fire()
    {

        Vector3 originalVector = Quaternion.AngleAxis(angle * 0.5f, Vector3.forward) * rotatingPart.up;
        Vector3 rotatedVector;

        projTransfroms = new List<Transform>();

        for (int i = 0; i < _projectileCount; i++)
        {
            float randomness = Random.Range(-_projectileRandomness, _projectileRandomness);
            rotatedVector = Quaternion.AngleAxis(((-angle / (_projectileCount - 1)) * i) + randomness, Vector3.forward) * originalVector;
            Vector3 targetLocation = rotatingPart.position + (rotatedVector * structureSO.Range);

            Projectile prefab = projectilePrefabs[Random.Range(0, projectilePrefabs.Length)];

            Transform projOrigin = i==0? _projOrigin : proj2Origin;

            projTransfroms.Add(Projectile.CreateNewProjectile(prefab, projOrigin.position, targetLocation, _projectileSpeed).transform);

            if (i == 0) 
            { 
                //VEA.SetVector3("Start", _projOrigin.position);
                StartCoroutine(FollowProjectile(projTransfroms[0], VEA));

            }

            if (i == 1) 
            { 
                //VEB.SetVector3("Start", proj2Origin.position);
                StartCoroutine(FollowProjectile(projTransfroms[1], VEB));
            }


        }

        isTargeting = true;
        Damage();
        new Delay(this, _extensionDuration, () => isTargeting = false);

        endPointA = projTransfroms[0];
        endPointB = projTransfroms[1];

        StartCoroutine(DamagePulse());
    }

    IEnumerator FollowProjectile(Transform proj, VisualEffect VE)
    {
        VE.enabled = true;
        chainsOut = true;
        while (proj)
        {
            VE.SetVector3("Start", proj.position);
            yield return null;
        }

        while (isTargeting)
        {
            yield return null;
        }

        Vector3 startPos = VE.GetVector3("Start");
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * retractionTime;
            VE.SetVector3("Start", Vector3.Lerp(startPos, VE.gameObject.transform.position,t));
            yield return null;
        }
        if (VE == VEA)
        {
            animator.QueueAnimation("Reload");
            animator.QueueAnimation("Idle");
        }
        chainsOut = false;
        VE.enabled = false;
    }

    IEnumerator DamagePulse()
    {
        lightning.SetFloat("Spawn Rate", 50);
        yield return new WaitForSeconds(1);
        while (isTargeting)
        {
            Damage();

            yield return new WaitForSeconds(tickDuration);
        }
        lightning.SetFloat("Spawn Rate", 0);

    }

    void Damage()
    {
        Enemy[] enemies = GetEnemiesInRange();
        foreach (Enemy enemy in enemies)
        {
            if (PointInAngle(enemy.transform.position, transform.position, rotatingPart.up, angle))
            {
                bool died = enemy.TakeDamageInstant(structureSO.Damage);
                if (died)
                    Kills++;
            }
        }
    }
}
