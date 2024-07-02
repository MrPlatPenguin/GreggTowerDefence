using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public abstract class RapidFire : Structure
{
    protected abstract override float GetProjectileSpeed();
    private List<int> usedIndexes = new List<int>();
    protected Transform lastFiredArrow;
    [SerializeField] protected List<SpriteAnimator> arrowAnimators;
    [SerializeField] float _projectileSpeed = 20f;
    protected float projectileSpeed { get { return _projectileSpeed * MapGenerator.CellSize; } }

    protected override void Attack(float projSpeed = float.PositiveInfinity)
    {
        base.Attack(projSpeed);
        float delay = Vector2.Distance(transform.position, attackTarget.transform.position) / projectileSpeed;

        bool died = attackTarget.TakeDamageDelay(structureSO.Damage, delay);
        if (died)
            Kills++;
        PlayFireSound();
    }

    //given a length of a list this function will return a random number within range of that list.
    //It will never repeat numbers unless it has exhausted all other options.
    protected int GetAllRandomIndex(int listLength)
    {
        listLength--;
        int randomIndex = Random.Range(0, listLength);
        int coinFlip = Random.Range(0, 1);
        while (usedIndexes.Contains(randomIndex))
        {
            if (coinFlip == 0)
            {
                randomIndex = randomIndex - 1 == -1 ? listLength : randomIndex - 1;
            }
            else
            {
                randomIndex = randomIndex + 1 == listLength + 1 ? 0 : randomIndex + 1;
            }
        }
        lastFiredArrow = arrowAnimators[randomIndex].transform;
        return randomIndex;
    }
}
