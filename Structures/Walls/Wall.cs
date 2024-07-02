using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Wall : Structure
{
    [Range(0f,1f)]
    [SerializeField] float thornsDamage;
    [SerializeField] SpriteRenderer spriteRenderer;
    Sprite defaultTexture;
    [SerializeField] Sprite topTexture;

    protected override void Awake()
    {
        base.Awake();

        defaultTexture = spriteRenderer.sprite;

        ConnectVerticalTextures();
    }

    void ConnectVerticalTextures()
    {
        MapGenerator.Grid.GetXY(transform.position, out int x, out int y);

        WorldObject northWorldObject = MapGenerator.Grid.GetGridObject(x, y + 1).WorldObject;
        if (northWorldObject != null && northWorldObject is Wall)
            ((Wall)northWorldObject).spriteRenderer.sprite = ((Wall)northWorldObject).topTexture;


        WorldObject southWorldObject = MapGenerator.Grid.GetGridObject(x, y - 1).WorldObject;
        if (southWorldObject != null && southWorldObject is Wall)
            spriteRenderer.sprite = topTexture;

        OnDestroy += DisconnectVerticalTextures;
    }

    void DisconnectVerticalTextures()
    {
        MapGenerator.Grid.GetXY(transform.position, out int x, out int y);

        WorldObject northWorldObject = MapGenerator.Grid.GetGridObject(x, y + 1).WorldObject;
        if (northWorldObject != null && northWorldObject is Wall)
            ((Wall)northWorldObject).spriteRenderer.sprite = ((Wall)northWorldObject).defaultTexture;
    }

    protected override Enemy TryGetAttackTarget()
    {
        return null;
    }

    public override void TakeDamage(float damage, Enemy attacker)
    {
        base.TakeDamage(damage, attacker);
        bool died = attacker.TakeDamageInstant(damage * thornsDamage);
        if (died)
            Kills++;
    }

    protected override float GetProjectileSpeed()
    {
        throw new System.NotImplementedException();
    }
    protected override void RunAttackSquence()
    {
    }
}