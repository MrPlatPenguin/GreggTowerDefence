using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IPlayerAttackable
{
    public EnemySO enemySO;
    float _currentHealth;
    [SerializeField] Slider _healthBar;
    [SerializeField] SpriteAnimator animator;

    [field:SerializeField] public Boids BoidAI { get; private set; }
    [SerializeField] LayerMask attackLayers;
    [SerializeField] DamageFlash _damageFlash;
    bool hasDied = false;
    bool attackedThisJump = false;

    [SerializeField] SlimeDeath deathPrefab;
    public event Action OnDeath;

    [Header("SFX")]
    [SerializeField] SoundClip _deathSound;

    static QuadTree enemyQuadTree;
    QuadTree locationQuadTree;

    float attackFrequency = 0.2f;
    float timeSinceLastAttackAttempt;

    float speculativeHealth;

    // If an enemy is not in the quad tree it can't be found by a tower.
    bool inQuadTree = false;

    static List<Enemy> _enemyPool = new List<Enemy>();

    [SerializeField] RainbowSlime rainbow;

    private void Update()
    {
        if (!BoidAI.IsGrounded && !attackedThisJump)
        {
            timeSinceLastAttackAttempt += Time.deltaTime;
            TryAttack();
        }
        else if (BoidAI.IsGrounded)
            attackedThisJump = false;

        UpdateQuadTree();
    }

    void TryAttack()
    {
        if (timeSinceLastAttackAttempt > attackFrequency)
        {
            timeSinceLastAttackAttempt = 0f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, BoidAI.Velocity.normalized, enemySO.AttackRange, attackLayers);
            //Debug.DrawRay(transform.position, BoidAI.Velocity.normalized * enemySO.AttackRange, Color.red);

            if (hit.collider != null && hit.collider.TryGetComponent(out IDamageReciver target))
                Attack(target);
        }
    }

    void Attack(IDamageReciver target)
    {
        target.TakeDamage(enemySO.Damage, this);
        attackedThisJump = true;
    }

    public static QuadTree GetEnemyQuadTree()
    {
        return enemyQuadTree;
    }

    public void Die()
    {
        // Protect against this being called twice
        if (hasDied)
            return;

        RemoveFromQuadTree();

        SlimeDeath deathAnim = Instantiate(deathPrefab, transform.position, transform.rotation);
        deathAnim.SetColor(enemySO.color);
        Vector3 scale = deathAnim.transform.localScale;
        Vector3 spriteScale = animator.GetSpriteRenederer().transform.localScale;
        deathAnim.transform.localScale = new Vector3(scale.x * spriteScale.x, scale.y * spriteScale.y, scale.z * spriteScale.z);

        hasDied = true;
        FloatingText.Create("+" + enemySO.GoldDrop.ToString() + "g", transform.position, Color.yellow, 1f, 0.5f, 13);
        SoundManager.PlaySound(_deathSound, transform, false);

        GoldManager.PlayerGold.AddGold(enemySO.GoldDrop);
        ScoreManager.AddScore(enemySO.GoldDrop * 6, ScoreManager.Category.Kill);
        OnDeath?.Invoke();
        PoolEnemy();
    }

    public bool TakeDamageDelay(float damage, float delay)
    {
        if (gameObject == null || damage <= 0)
            return false;

        if (delay <= 0)
            return TakeDamageInstant(damage);

        speculativeHealth -= damage;
        StartCoroutine(DamageDelay(damage, delay));

        if (speculativeHealth <= 0)
            RemoveFromQuadTree();

        return speculativeHealth <= 0;
    }

    public bool TakeDamageInstant(float damage, bool updateSpecHealth = true)
    {
        if (gameObject == null || damage <= 0)
            return false;

        _damageFlash.Flash(0.2f);
        _healthBar.gameObject.SetActive(true);
        _currentHealth -= damage;
        if (updateSpecHealth)
            speculativeHealth -= damage;
        _healthBar.value = _currentHealth / enemySO.Health;
        if (_currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    IEnumerator DamageDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        TakeDamageInstant(damage, false);

    }

    public void KnockBack(Vector2 origin, float force)
    {
        Vector2 direction = (Vector2)transform.position - origin;
        BoidAI.rb.AddForce(direction * force * MapGenerator.CellSize);
        BoidAI.Disable(0.5f);
    }

    public void AttackFromPlayer(WeaponSO weapon)
    {
        TakeDamageInstant(weapon.EnemyDamage);
        KnockBack(transform.position, weapon.Knockback);
    }

    public static void CreateQuadTree()
    {
        enemyQuadTree = new QuadTree(new Rect(0, 0, MapGenerator.Grid.Width * MapGenerator.Grid.CellSize, MapGenerator.Grid.Height * MapGenerator.Grid.CellSize), 5);
    }

    void AddToQuadTree()
    {
        if (inQuadTree)
            return;

        locationQuadTree = enemyQuadTree.Insert(this);
        inQuadTree = true;
    }

    void RemoveFromQuadTree()
    {
        if (!inQuadTree)
            return;

        UpdateQuadTree();

        enemyQuadTree.Remove(this);
        inQuadTree = false;
    }

    void UpdateQuadTree()
    {
        locationQuadTree = enemyQuadTree.UpdateDirty(this, locationQuadTree);
    }

    public Enemy SpawnEnemy(EnemySO enemy, Vector2 position)
    {
        Enemy newEnemy;

        if (_enemyPool.Count == 0)
        {
            newEnemy = Instantiate(this, position, Quaternion.identity);
        }

        else
        {
            newEnemy = _enemyPool[0];
            _enemyPool.RemoveAt(0);
            if (newEnemy == null)
            {
                Debug.Log("Null enemy skipped");
                return SpawnEnemy(enemy, position);
            }
            newEnemy.gameObject.SetActive(true);
            newEnemy.transform.position = position;
            newEnemy.BoidAI.ResetAI();
            newEnemy.animator.ResetAnimator();
            newEnemy._damageFlash.CancelFlash();
            newEnemy.hasDied = false;
            newEnemy._healthBar.gameObject.SetActive(false);
        }

        if (enemy == null)
            return newEnemy;

        newEnemy.enemySO = enemy;
        newEnemy.rainbow.enabled = newEnemy.enemySO.name == "8 RainbowSlime";
        newEnemy.BoidAI.SetSpeed(enemy.Speed);
        newEnemy.animator.GetSpriteRenederer().color = enemy.color;
        newEnemy._currentHealth = enemy.Health;
        newEnemy.speculativeHealth = enemy.Health;
        newEnemy.AddToQuadTree();
        newEnemy._damageFlash.SetStartColors();

        return newEnemy;
    }

    void PoolEnemy()
    {
        gameObject.SetActive(false);
        _enemyPool.Add(this);
    }

    public static int PooledEnemyCount()
    {
        return _enemyPool.Count;
    }

    private void OnDrawGizmosSelected()
    {
        locationQuadTree.DrawQuad(Color.yellow, true);
    }
}
