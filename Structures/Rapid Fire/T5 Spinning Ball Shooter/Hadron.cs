using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Hadron : RapidFire
{
    //[SerializeField] VisualEffect lightning;
    [SerializeField] float maxSpin;
    [SerializeField] float rotSpeed;
    [SerializeField] GameObject arms;
    [SerializeField] float randomRange;
    [SerializeField] float offset;
    float rotVelocity = 0;

    [Header("Smudge")]
    [SerializeField] SpriteRenderer spinner;
    [SerializeField] SpriteRenderer smudge;
    [SerializeField] float _fadeInSpeed;
    [SerializeField] bool _spunUp;
    [SerializeField] Projectile projectile;

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        if (_spunUp)
        {
            base.Attack(projectileSpeed);

            Vector2 randomPos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * randomRange;
            randomPos += (((Vector2)transform.position) - (Vector2)attackTarget.transform.position).normalized * offset;

            Projectile.CreateNewProjectile(projectile, (Vector2)transform.position + randomPos, attackTarget.transform.position, projectileSpeed);
        }
    }

    protected override void Update()
    {
        base.Update();

        CheckSpinUp();
    }

    void CheckSpinUp()
    {
        float prevVel = rotVelocity;

        if (attackTarget == null)
        {
            rotVelocity -= rotSpeed * Time.deltaTime;
        }
        else
        {
            rotVelocity += rotSpeed * Time.deltaTime;
        }

        if (prevVel == rotVelocity)
            return;

        rotVelocity = Mathf.Clamp(rotVelocity, 0, maxSpin);
        arms.transform.rotation *= Quaternion.Euler(0, 0, rotVelocity);

        Color color = smudge.color;

        color.a = Mathf.Lerp(0, 0.5f, rotVelocity / maxSpin);
        smudge.color = color;
        color.a = Mathf.Lerp(1f, 0.5f, rotVelocity / maxSpin);
        spinner.color = color;
        _spunUp = rotVelocity >= maxSpin;
    }

    protected override float GetProjectileSpeed()
    {
        return Mathf.Infinity;
    }
}
