using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : WorldObject, IPlayerAttackable
{
    [SerializeField] ResourceSO resourceSO;
    [HideInInspector] public ResourceTier Tier;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] DamageFlash damageFlash;
    float _currentHealth;
    [SerializeField] BoxCollider2D collider;
    static List<Resource> pooledResources = new List<Resource>();
    TileObject tileObject;

    private void Awake()
    {
        pooledResources.Add(this);
    }

    void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        DropResources(Mathf.Clamp01(damage / resourceSO.Health));

        if (_currentHealth <= 0)
        {
            PoolResource();
            CameraController.Shake(0.2f, 100f);
        }
        else
            CameraController.Shake(0.2f, 35f);
    }

    private void OnDestroy()
    {
        pooledResources.Remove(this);
    }

    void DropResources(float damage) 
    {
        int amount = Mathf.RoundToInt(Random.Range(resourceSO.MinDropAmount, resourceSO.MaxDropAmount + 1) * damage);

        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * MapGenerator.CellSize * 0.5f;
            Instantiate(resourceSO.ItemPrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
        }
    }

    public void SetResource(ResourceSO resource, TileObject tile)
    {
        if (resource == null)
        {
            //PoolResource();
            return;
        }
        else
            gameObject.SetActive(true);

        resourceSO = resource;
        Tier= resource.Tier;
        _spriteRenderer.sprite = resource.sprite;
        _currentHealth = resource.Health;
        tileObject = tile;

        // Fill the tile so the player can't escape the map then destroy itself so player can't interact with it
        if (resource.name == "Void")
        {
            Vector3 newBounds = collider.bounds.size;
            newBounds.y = newBounds.x;
            collider.size = newBounds;
            collider.offset = Vector3.zero;
            Destroy(this);
            return;
        }
    }

    public void AttackFromPlayer(WeaponSO weapon)
    {
        if (Tier <= weapon.MaxTier)
        {
            Flash();
            TakeDamage(weapon.ResourceDamage);
        }

        else
            FloatingText.Create("Too Hard!", transform.position, Color.red);

        if (resourceSO.HitSound != null)
            SoundManager.PlaySound(resourceSO.HitSound, transform, false);
    }

    void Flash()
    {
        damageFlash.Flash(0.1f);
    }

    void PoolResource()
    {
        Destroy(gameObject);
        //gameObject.SetActive(false);
        //pooledResources.Add(this);
        //tileObject.SetWorldObject<WorldObject>(null);
        //tileObject = null;
    }

    public static List<Resource> GetResourcesPool()
    {
        return pooledResources;
    }

    public static void ClearResourcesPool()
    {
        for (int i = 0; i < pooledResources.Count; i++)
        {
            Destroy(pooledResources[i].gameObject);
        }
        pooledResources.Clear();
    }

    public static Resource DepoolResource()
    {
        Debug.Log(pooledResources.Count + "Resource depooled");
        Resource resource = pooledResources[0];
        pooledResources.RemoveAt(0);
        return resource;
    }
}