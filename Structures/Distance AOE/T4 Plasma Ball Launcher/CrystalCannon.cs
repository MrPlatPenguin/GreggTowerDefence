using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CrystalCannon : Catapult
{
    [Header("Gun")]
    [SerializeField] Transform _gunTransform;
    [SerializeField] SpriteAnimator _gunAnim;
    [SerializeField] Projectile _lazerProj;
    [SerializeField] float _lazerProjSpeed;
    [SerializeField] float _deathDelay;
    [SerializeField] SoundClip _lazerSound;

    protected override void Awake()
    {
        base.Awake();
        OnProjHit += FireGun;
    }

    protected override void Update()
    {
        base.Update();

        if (proj != null)
        {
            _gunTransform.up = (proj.position - transform.position).normalized;
        }
    }

    void FireGun(Vector3 point)
    {
        _gunAnim.SetAnimation("LazerFire");
        _gunAnim.QueueAnimation("LazerIdle");
        Projectile.CreateNewProjectile(_lazerProj, transform.position, point, _lazerProjSpeed);
        SoundManager.PlaySound(_lazerSound, _gunTransform);
    }
}
