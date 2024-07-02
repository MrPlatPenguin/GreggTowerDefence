using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Structure : WorldObject, IDamageReciver
{
    //declaring variables

    public StructureSO structureSO;
    float _maxHealth { get { return structureSO.MaxHealth; } }
    public float CurrentHealth { get; protected set; }
    public new string name { get => structureSO.Name; }

    int _kills;
    public int Kills
    {
        get => _kills;
        protected set
        {
            OnKill?.Invoke(value);
            _kills = value;
        }
    }

    protected Enemy attackTarget { get; private set; }
    protected LayerMask enemyLayer { get { return LayerMask.GetMask("Enemy"); } }

    [SerializeField] protected Transform rotatingPart;
    protected float timeSinceLastAttack = 0;
    [HideInInspector] public TileObject tile;

    LineRenderer _rangeCircle;

    public event Action OnHealthChange;
    public event Action OnDestroy;
    public event Action OnUpgrade;
    public delegate void DOnKill(int kills);
    public DOnKill OnKill;
    [SerializeField] protected SpriteAnimator animator;
    [SerializeField] Slider healthBar;

    public static Structure SelectedStructure { get; private set; }
    [SerializeField] Structure debris;

    [Header("SFX")]
    [SerializeField] protected SoundClip[] _fireSounds;
    [SerializeField] SoundClip _destroySound;

    [SerializeField] ItemArray _items;

    [Header("VFX")]
    [SerializeField] DamageFlash flashScript;

    public static int attackPassesNew;
    public static float totalTimeNew;

    public static int attackPassesOld;
    public static float totalTimeOld;

    private IEnumerator healthCoroutine;
    private bool healthCoRun = false;
    public static bool hasUpgraded = false;

    protected virtual void Awake()
    {
        //Imediately repair the tower to full health
        timeSinceLastAttack = 1000;
        RepairStructure();

        SubscribeEvents();

        gameObject.AddComponent<MouseOverHighlight>();
        OnHealthChange += UpdateHealthBar;
    }

    protected virtual void AnimationQueue()
    {
        animator.ClearQueue();
        animator.SetAnimation("Fire");
        animator.QueueAnimation("Idle Unloaded");
        animator.QueueAnimation("Reload");
        animator.QueueAnimation("Idle");
    }

    protected virtual void Update()
    {
        //Constantly try to attack a target.
        RunAttackSquence();
    }

    protected virtual void PlayFireSound()
    {
        if (_fireSounds == null || _fireSounds.Length <= 0)
            return;
        int i = UnityEngine.Random.Range(0, _fireSounds.Length);
        SoundManager.PlaySound(_fireSounds[i], transform, true);
    }

    /// <summary>
    /// Looks for an enemy in range and points the turret at the nearest one.
    /// Also runs the attack function if an enemy is found.
    /// </summary>
    protected virtual void RunAttackSquence()
    {
        timeSinceLastAttack += Time.deltaTime;
        //Get the attack target first so we can point the turret at it.
        attackTarget = TryGetAttackTarget();
        if (attackTarget == null)
            return;

        RotateTurret(attackTarget);

        //Checks the cooldown
        if (timeSinceLastAttack < structureSO.TimeBetweenAttacks)
        {
            return;
        }

        //If found target attacks it
        Attack(GetProjectileSpeed());
    }

    protected abstract float GetProjectileSpeed();

    /// <summary>
    /// Currently just plays the attack sound and resets the cooldown.
    /// Is virtual so we can add onto this from other objects.
    /// </summary>
    /// <param name="targets"></param>
    /// 

    protected virtual void Attack(float projSpeed = Mathf.Infinity)
    {
        timeSinceLastAttack = 0;
    }

    /// <summary>
    /// Returns all enemies in range of the turret
    /// Range is set in the StructureSO
    /// </summary>
    /// <returns></returns>
    protected Enemy[] GetEnemiesInRange()
    {
        return GetEnemiesInRadius(structureSO.Range);
    }

    /// <summary>
    /// Gets all enemies in a r=float radius centered on the tower transform.
    /// </summary>
    /// <param name="radius"></param>
    /// <returns>Enemies in radius of tower.</returns>
    protected Enemy[] GetEnemiesInRadius(float radius)
    {
        return GetEnemiesInRadius(radius, transform.position);
    }

    /// <summary>
    /// Gets enemies in a r=float radius centered around a Vector 2 Position.
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected Enemy[] GetEnemiesInRadius(float radius, Vector2 position)
    {
        return Enemy.GetEnemyQuadTree().EnemiesInRadius(position, radius);
    }

    protected Enemy[] GetEnemiesInRadiusOld(float radius, Vector2 position)
    {
        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(position, radius);
        List<Enemy> enemies = new List<Enemy>();
        foreach (Collider2D collider in objectsFound)
        {
            if (collider.gameObject.CompareTag("Enemy") && collider.TryGetComponent(out Enemy enemy) && !enemies.Contains(enemy))
                enemies.Add(enemy);
        }

        return enemies.ToArray();
    }

    /// <summary>
    /// Damages all enemies for the towers damage value (set in StructureSO)
    /// </summary>
    /// <param name="enemies"></param>
    protected void DamageEnemies(Enemy[] enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            bool died = enemy.TakeDamageInstant(structureSO.Damage);
            if (died)
                Kills++;
        }
    }

    /// <summary>
    /// Returns the enemey closest to a Vector2 position.
    /// </summary>
    /// <param name="enemies"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    protected Enemy ClosestEnemyToCentre(Enemy[] enemies)
    {
        return ClosestEnemy(enemies, MapGenerator.MapCentre);
    }

    protected Enemy ClosestEnemy(Enemy[] enemies, Vector2 point)
    {
        float closestDistance = Mathf.Infinity;
        Enemy closestEnemy = null;
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    protected Enemy FurthestEnemy(Enemy[] enemies, Vector2 point)
    {
        float furthestDistance = 0;
        Enemy furthestEnemy = null;
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, point);
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestEnemy = enemy;
            }
        }
        return furthestEnemy;
    }

    /// <summary>
    /// Function run during attack sequence when scanning for enemies.
    /// </summary>
    /// <returns>Should return the enemy the tower wishes to target.</returns>
    protected virtual Enemy TryGetAttackTarget()
    {
        Enemy[] enemies = GetEnemiesInRange();

        if (enemies.Length > 0)
            return ClosestEnemyToCentre(enemies);
        else
            return null;
    }

    /// <summary>
    /// Destroy's the structure, unsubscribes it from events and returns the grid space to normal.
    /// </summary>
    public void DestroyStructure(bool placeDebris = true)
    {
        float sellRefund = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(CurrentHealth / structureSO.MaxHealth));


        for (int i = 0; i < structureSO.Cost.Wood * sellRefund; i++)
        {
            SpawnResource(_items.Wood, true).SetDontCountToScore();
        }
        for (int i = 0; i < structureSO.Cost.Stone * sellRefund; i++)
        {
            SpawnResource(_items.Stone, true).SetDontCountToScore();
        }
        for (int i = 0; i < structureSO.Cost.Metal * sellRefund; i++)
        {
            SpawnResource(_items.Metal, true).SetDontCountToScore();
        }
        for (int i = 0; i < structureSO.Cost.Crystal * sellRefund; i++)
        {
            SpawnResource(_items.Crystal, true).SetDontCountToScore();
        }
        for (int i = 0; i < structureSO.Cost.DarkMetal * sellRefund; i++)
        {
            SpawnResource(_items.DarkMetal, true).SetDontCountToScore();
        }

        OnDestroy?.Invoke();
        UnsubscribeEvents();
        SoundManager.PlaySound(_destroySound,transform, false);

        if (placeDebris)
        {
            Structure debrisInstance = tile.CreateNewWorldObject<Structure>(debris);
            debrisInstance.tile = tile;
        }
        else
            tile.CreateNewWorldObject<Structure>(null);
    }


    /// <summary>
    /// Runs DestroyStructure(); after refunding the player.
    /// </summary>
    public void SellStructure()
    {
        if (!structureSO.CanBeSold)
            return;

        DestroyStructure();
    }

    /// <summary>
    /// Takes the "rotating part" of the turret and points it towards an enemy target
    /// </summary>
    /// <param name="target"></param>
    protected virtual void RotateTurret(Enemy target)
    {
        if (target == null || rotatingPart == null)
            return;
        Vector3 targetRot = target.transform.position - rotatingPart.position;
        targetRot.z = 0f;
        rotatingPart.up = Vector3.Lerp(rotatingPart.up, targetRot, 0.05f);
    }

    /// <summary>
    /// Reduces structure health and invokes a structure update check.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attacker"></param>
    public virtual void TakeDamage(float damage, Enemy attacker)
    {
        CurrentHealth -= damage;

        OnHealthChange?.Invoke();

        //flash here
        flashScript.Flash(0.1f);

        if (CurrentHealth <= 0)
        {
            DestroyStructure();
        }
    }

    public void SetDelays(Enemy[] targets, float projectileSpeed)
    {
        Transform previousEnemy = transform;

        float totalDelay = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                float delay = Vector2.Distance(previousEnemy.position, targets[i].transform.position) / projectileSpeed;

                delay += totalDelay;
                totalDelay += delay - totalDelay;

                previousEnemy = targets[i].transform;

                Enemy target = targets[i];

                bool died = target.TakeDamageDelay(structureSO.Damage, delay);
                if (died)
                    Kills++;
            }
        }
    }


    /// <summary>
    /// Heals the structure for the specified amount, clamped to max structure health
    /// </summary>
    /// <param name="amount"></param>
    public virtual void Heal(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, _maxHealth);

        OnHealthChange?.Invoke();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            if (healthCoRun == true)
            {
                StopCoroutine(healthCoroutine);
            }

            float target = Mathf.Clamp01(CurrentHealth / _maxHealth);

            healthCoroutine = LerpHealth(target, healthBar.value, 0.2f);
            StartCoroutine(healthCoroutine);


            //healthBar.value = Mathf.Clamp01(CurrentHealth / _maxHealth);
        }
    }

    IEnumerator LerpHealth(float target, float start, float time)
    {
        healthCoRun = true;
        float t = 0;

        while (t <= time)
        {
            t += Time.deltaTime;
            healthBar.value = Mathf.Lerp(start, target, t / time);
            healthBar.gameObject.SetActive(healthBar.value < 1);
            yield return null;
        }
        healthCoRun = false;
    }

    /// <summary>
    /// Swaps the structure out for it's upgraded varient.
    /// </summary>
    public virtual void Upgrade()
    {
        // Checks if the structure has an upgrade
        if (structureSO.Upgrade == null)
            return;

        UnsubscribeEvents();

        // Replaces the old structure with the new one
        Structure newStructure = tile.CreateNewWorldObject(structureSO.Upgrade);
        newStructure.tile = tile;
        newStructure.Kills = Kills;

        // Charges the player for the upgrade
        ResourceManager.playerResources.Subtract(structureSO.UpgradeCost);

        // Opens the menu for the new structure
        if (Builder.BuildMode == BuildMode.None)
            newStructure.Select();

        hasUpgraded = true;
    }


    /// <summary>
    /// Fully heals a tower to max health
    /// </summary>
    void RepairStructure()
    {
        Heal(Mathf.Infinity);
    }

    /// <summary>
    /// Subscribes tower functions so that it can repair at the end of the day and show visable damage.
    /// </summary>
    void SubscribeEvents()
    {
        GameManager.OnDayChange += RepairStructure;
        //OnHealthChange += UpdateSprite;
    }

    /// <summary>
    /// Unsubscribes tower functions.
    /// </summary>
    void UnsubscribeEvents()
    {
        GameManager.OnDayChange -= RepairStructure;
        //OnHealthChange -= UpdateSprite;
    }

    /// <summary>
    /// Inherits from WorldObject to check why the player clicked on the tower. Runs the necesary functions.
    /// </summary>
    /// <param name="tileObject"></param>
    public override void OnClickStart(TileObject tileObject)
    {
        switch (Builder.BuildMode)
        {
            case BuildMode.None:
                Select();
                break;
            case BuildMode.Build:
                break;
            case BuildMode.Upgrade:
                if (ResourceManager.playerResources.CanAfford(structureSO.UpgradeCost))
                    Upgrade();
                break;
            case BuildMode.Destroy:
                SellStructure();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Runs the OnStructureSelected event and draws the range circle.
    /// </summary>
    public void Select()
    {
        SelectedStructure = this;
        StructureEvents.OnStructureSelected?.Invoke(this);
        _rangeCircle = gameObject.DrawCircle(structureSO.Range, 50, "Structure Range Circle");
    }

    /// <summary>
    /// Sets selectedStructure to null and stops drawing the range circle.
    /// </summary>
    public void Deselect()
    {
        SelectedStructure = null;
        if (_rangeCircle != null)
            CircleRenderer.DestroyCircle(_rangeCircle);
        StructureEvents.OnStructreDeselected?.Invoke();
    }

    /// <summary>
    /// Exclusively shows the cost of the tower when hovered over.
    /// </summary>
    private void OnMouseEnter()
    {
        switch (Builder.BuildMode)
        {
            case BuildMode.None:
                break;
            case BuildMode.Build:
                break;
            case BuildMode.Upgrade:
                if (structureSO.HasUpgrade)
                    ResourceNumbers.ShowCost(structureSO.UpgradeCost);
                break;
            case BuildMode.Destroy:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Stops showing the cost of the tower when the mouse leaves it.
    /// </summary>
    private void OnMouseExit()
    {
        switch (Builder.BuildMode)
        {
            case BuildMode.None:
                break;
            case BuildMode.Build:
                break;
            case BuildMode.Upgrade:
                ResourceNumbers.HideCost();
                break;
            case BuildMode.Destroy:
                break;
            default:
                break;
        }
    }

    protected bool PointInAngle(Vector2 point, Vector2 origin, Vector2 forward, float angle)
    {
        Vector2 direction = point - origin;

        return Vector2.Angle(forward.normalized, direction.normalized) < angle;
    }

    protected ItemResource SpawnResource(ItemResource resource, bool isPersistant)
    {
        ItemResource itemInstance = Instantiate(resource, transform.position, Quaternion.identity);
        itemInstance.isPersistant = isPersistant;
        itemInstance.transform.position += UnityEngine.Random.insideUnitSphere * MapGenerator.CellSize;
        return itemInstance;
    }

    public void SetHealthBar(Slider newHealthBar)
    {
        healthBar = newHealthBar;
    }
}