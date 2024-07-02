using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDamageReciver
{
    #region Statics
    static Player instance;
    public static WeaponSO CurrentWeapon { get => instance._weapons[instance._currentWeaponIndex]; }
    /// <summary>
    /// Returns -1 if there is no upgrade.
    /// </summary>
    public static int WeaponUpgradeCost { get => instance._currentWeaponIndex < instance._weapons.Length - 1 ? instance._weapons[instance._currentWeaponIndex + 1].Cost : -1; }
    public static bool InBase { get => instance._inBase; }
    public static bool AtHome { get => CharacterController.Instance.GetCurrentBiome().Name == "Home"; }
    public static Transform Transform { get => instance.transform; }
    public static float MaxHealth { get; private set; }
    public static bool IsCritialyWounded { get => instance._isCriticallyWounded; }
    #endregion

    [field: SerializeField] public float Health { get; private set; }

    [SerializeField] LayerMask _hitLayers;

    [SerializeField] WeaponSO[] _weapons;
    int _currentWeaponIndex;

    [SerializeField] float _nightTimeDamage;
    [SerializeField] float _nightTimeTimeBetweenTicks;
    float _nightTimer;

    [SerializeField] float _attackCooldown;

    bool _inBase;
    bool _isCriticallyWounded;

    [Header("SFX")]
    [SerializeField] SoundClip attackSound;
    [SerializeField] SoundClip[] _damageSounds;

    [Header("VFX")]
    [SerializeField] Animator _axeAnim;
    [SerializeField] Transform _axeTransform;
    [SerializeField] DamageFlash _flashVFX;

    private void Awake()
    {
        instance = this;
        MaxHealth = Health;
        PlayerEvents.OnPlayerSpawn?.Invoke(this);
        PlayerEvents.OnEnterBase += delegate { _inBase = true; };
        PlayerEvents.OnExitBase += delegate { _inBase = false; };
        PlayerEvents.OnHealthChange += (float health, float damamge) => _isCriticallyWounded = health / MaxHealth < 0.25f;
    }

    private void Update()
    {
        if (_attackCooldown > 0)
            _attackCooldown -= Time.deltaTime;


        bool canAttack = Input.GetButton("Attack") &&
            !EventSystem.current.IsPointerOverGameObject() &&
            _attackCooldown <= 0 &&
            !Builder.IsBuilding &&
            !_inBase;

        if (canAttack)
            Attack();

        bool outAtNight = PlayerSmokeScript.InDark && !InBase;

        _nightTimer += Time.deltaTime;
        if (outAtNight && _nightTimer >= _nightTimeTimeBetweenTicks)
        {
            TakeDamage(_nightTimeDamage);
            _nightTimer = 0;
        }
        if (InBase)
        {
            Heal(10f * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage, Enemy attacker = null)
    {
        if (InBase)
            return;
        bool significantDamage = damage > 1f;
        Health = Health > 10f && Health - damage <= 0 ? 1f : Health - damage;
        PlayerEvents.OnHealthChange?.Invoke(Health, damage);
        if (significantDamage)
        {
            PlayerEvents.OnTakeDamage();
            _flashVFX.Flash(0.1f);
            SoundManager.PlaySound(_damageSounds[UnityEngine.Random.Range(0, _damageSounds.Length)], transform, false);
        }
        if (Health <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        float preHealth = Health;
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
        PlayerEvents.OnHealthChange?.Invoke(Health, Health- preHealth);
    }

    void Die()
    {
        GameManager.GameOver("You died");
    }

    void Attack()
    {
        Vector2 mouseDirection = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        // Axe Swing
        AxeSwing(mouseDirection);

        // Play Sound
        SoundManager.PlaySound(attackSound, transform);

        // Sets the cooldown of the attack
        _attackCooldown = CurrentWeapon.Cooldown;

        // Slows the player for the duration of the attack
        CharacterController.Instance.SetIsAttacking(_attackCooldown);
        mouseDirection.Normalize();

        //Draw debugs
        Debug.DrawRay(transform.position, mouseDirection * CurrentWeapon.Range, Color.magenta, CurrentWeapon.Cooldown);
        Debug.DrawRay(transform.position, mouseDirection.Rotate(CurrentWeapon.Angle) * CurrentWeapon.Range, Color.magenta, 1f);
        Debug.DrawRay(transform.position, mouseDirection.Rotate(-CurrentWeapon.Angle) * CurrentWeapon.Range, Color.magenta, 1f);

        new Delay(this, CurrentWeapon.Cooldown * 0.25f, () => DamageEnemies(mouseDirection));
    }

    void DamageEnemies(Vector2 mouseDirection)
    {
        //Get all the colliders in range on the hit layers
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, CurrentWeapon.Range, _hitLayers);

        List<IPlayerAttackable> targets = new List<IPlayerAttackable>();

        for (int i = 0; i <= CurrentWeapon.Angle; i++)
        {
            foreach (Collider2D collider in colliders)
            {
                //Check if the objects are within the angle of the weapons attack
                Vector2 enemyDirection = collider.transform.position - transform.position;
                enemyDirection.Normalize();
                float angle = Vector2.SignedAngle(mouseDirection, enemyDirection);
                if (Mathf.Abs(angle) > CurrentWeapon.Angle)
                    continue;

                //Attacks the target and adds them to a list so they don't get attacked multiple times if they have multiple colliders
                if (collider.TryGetComponent(out IPlayerAttackable target) && !targets.Contains(target))
                {
                    target.AttackFromPlayer(CurrentWeapon);
                    targets.Add(target);
                }
            }
        }

    }

    void AxeSwing(Vector2 mouseDirection)
    {
        _axeAnim.speed = 1 / CurrentWeapon.Cooldown;
        _axeAnim.SetTrigger("Swing");
        _axeTransform.up = mouseDirection;
        _axeTransform.localPosition = Vector2.zero + (mouseDirection.normalized * 0f);
    }

    public static bool CanAffordAxeUpgrade()
    {
        if (instance._currentWeaponIndex >= instance._weapons.Length - 1)
            return false;

        return WeaponUpgradeCost <= GoldManager.PlayerGold.GetGold();
    }

    public static int UpgradeAxe()
    {
        GoldManager.PlayerGold.Spend(WeaponUpgradeCost);
        instance._currentWeaponIndex++;
        PlayerEvents.OnUpgradeAxe?.Invoke();
        return instance._currentWeaponIndex;
    }

    public static void SetAxeLevel(int value)
    {
        instance._currentWeaponIndex = value;
        PlayerEvents.OnUpgradeAxe?.Invoke();
    }

    public static int GetAxeLevel() { return instance._currentWeaponIndex; }
}
