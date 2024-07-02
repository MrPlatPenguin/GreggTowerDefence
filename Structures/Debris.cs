using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : Structure
{
    [SerializeField] float timer = 3f;
    [SerializeField] SpriteRenderer SR;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Burn(timer * 0.6f, timer));
        //Invoke("Remove", timer);
    }

    protected override Enemy TryGetAttackTarget()
    {
        return null;
    }

    protected override void RunAttackSquence()
    {
    }

    protected override float GetProjectileSpeed()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Burn(float delay, float duration)
    {
        //yield return new WaitForSeconds(delay);
        float timePassed = 0f;

        while (timePassed < delay)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        float originalValue = SR.material.GetFloat("_Distance");

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            SR.material.SetFloat("_Distance", Mathf.Lerp(originalValue, 0, (timePassed - delay) / (duration - delay)));

            yield return null;
        }

        Remove();
    }

    void Remove()
    {
        DestroyStructure(false);
    }
}
