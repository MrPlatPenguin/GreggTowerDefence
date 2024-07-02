using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class BallLauncher : SingleShot
{
    [SerializeField] TrackingProjectile _projectile;
    [SerializeField] float _maxRicochetRange = 5f;
    float maxRicochetRange { get { return _maxRicochetRange * MapGenerator.CellSize; } }

    protected override void AnimationQueue()
    {
        animator.ClearQueue();
        animator.SetAnimation("Fire");
        animator.QueueAnimation("Idle Ball");
    }

    protected override void GetRemainingTargets(out List<Enemy> targets, out List<Transform> targetTransforms)
    {
        targets = new List<Enemy>() { attackTarget };
        targetTransforms = new List<Transform>();

        Vector2 prevTargetPos = attackTarget.transform.position;
        targetTransforms.Add(attackTarget.transform);

        for (int i = 0; i < maxTargets - 1; i++)
        {
            Enemy nextTarget = GetNextClosest(Enemy.GetEnemyQuadTree().EnemiesInRadius(prevTargetPos, maxRicochetRange), targets, prevTargetPos);
            if (nextTarget == null)
                break;
            targets.Add(nextTarget);
            targetTransforms.Add(nextTarget.transform);
            prevTargetPos = nextTarget.transform.position;
        }
    }

    Enemy GetNextClosest(Enemy[] targets, List<Enemy> alreadyTargeted, Vector2 position)
    {
        float currentDisctance = Mathf.Infinity;
        Enemy currentClosest = null;
        for (int i = 0; i < targets.Length; i++)
        {
            Enemy checkingEnemy = targets[i];
            if (Vector2.Distance(checkingEnemy.transform.position, position) <= currentDisctance && !alreadyTargeted.Contains(checkingEnemy))
            {
                currentClosest = checkingEnemy;
            }
        }
        return currentClosest;
    }

    protected override void LaunchProjectile(List<Transform> targets)
    {
        TrackingProjectile projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
        targets.Add(transform);
        targets.Insert(0, transform);
        projectile.Init(targets.ToArray(), projectileSpeed);
    }
}
