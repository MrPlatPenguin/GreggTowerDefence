using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class FlameThrower : CloseAOE
{
    [SerializeField] VisualEffect fire;
    bool isAttacking;
    float lastAttackTime;
    private float flameBuildup;
    [SerializeField] float flameBuildupTime = 1;
    [SerializeField] float flameDecayTime = 3;
    private bool increasing = false;
    Coroutine currentState;
    float timeSinceLastSound;

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        base.Attack(projSpeed);
        animator.ClearQueue();
    }

    protected override void Awake()
    {
        base.Awake();
        currentState = StartCoroutine(DecreaseFlameBuildup());
        fire.Stop();
    }

    protected override Enemy TryGetAttackTarget()
    {
        Enemy[] enemies = GetEnemiesInRange();

        if (enemies == null || enemies.Length == 0)
        {
            if (increasing) { StopCoroutine(currentState); currentState = StartCoroutine(DecreaseFlameBuildup()); }

            if (isAttacking)
            {
                animator.SetAnimation("Idle");
                fire.Stop();
            }
            isAttacking = false;
            return null;
        }

        if (!increasing) { StopCoroutine(currentState); currentState = StartCoroutine(IncreaseFlameBuildup()); }

        if (!isAttacking)
        {
            animator.SetAnimation("Fire");
            fire.Play();
            PlayFireSound();
        }
        isAttacking = true;
        if (Time.time - lastAttackTime > structureSO.TimeBetweenAttacks && flameBuildup == flameBuildupTime)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!PointInAngle(enemy.transform.position, transform.position, rotatingPart.up, angle))
                    continue;
                bool died = enemy.TakeDamageInstant(structureSO.Damage);
                if (died)
                    Kills++;
            }
            lastAttackTime = Time.time;
        }
        if(timeSinceLastSound > _fireSounds[0].clip.length)
        {
            timeSinceLastSound = 0;
            PlayFireSound();
        }
        timeSinceLastSound += Time.deltaTime;
        return enemies[0];
    }

    public override void Fire()
    {
        
    }

    private IEnumerator IncreaseFlameBuildup()
    {
        increasing = true;
        if (flameBuildup < 0) flameBuildup = 0;

        while (true)
        {
            flameBuildup += 1 * Time.deltaTime;
            if (flameBuildup > flameBuildupTime) flameBuildup = flameBuildupTime;
            yield return null;
        }
    }

    private IEnumerator DecreaseFlameBuildup()
    {
        increasing = false;
        while (true)
        {
            flameBuildup -= 1 * Time.deltaTime;
            if (flameBuildup < flameBuildupTime - flameDecayTime) flameBuildup = flameBuildupTime - flameDecayTime;
            yield return null;
        }
    }

}
