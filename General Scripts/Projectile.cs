using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool _initilized = false;
    float _speed;
    protected Vector2 _spawnPosition;
    bool destroyAtTarget;
    float age;
    [SerializeField] float maxLifeDuration = 5f;
    float maxDistance;

    public static Projectile CreateNewProjectile(Projectile projectilePreFab, Vector2 spawnPosition, Vector2 destination, float speed, bool destoryAtTarget = true)
    {
        if (projectilePreFab == null)
            return null;

        //create the projectile.
        Projectile projectile = Instantiate(projectilePreFab, spawnPosition, Quaternion.identity);

        projectile._spawnPosition = spawnPosition;
        projectile._speed = speed;
        projectile._spawnPosition = spawnPosition;
        Vector2 direction = destination - spawnPosition;
        projectile.maxDistance = direction.magnitude;
        projectile.destroyAtTarget = destoryAtTarget;
        direction = direction.normalized;
        projectile._initilized = true;

        projectile.StartCoroutine(projectile.Move(direction));

        return projectile;
    }

    private void Update()
    {
        if (!_initilized)
            CancelFire();

        age += Time.deltaTime;
        if (age >= maxLifeDuration)
            DestoryProjectile();

        if (destroyAtTarget && (transform.position - (Vector3)_spawnPosition).magnitude >= maxDistance)
            DestoryProjectile();
    }

    IEnumerator Move(Vector2 direction)
    {
        while (true)
        {
            transform.position += (Vector3)direction * _speed * Time.deltaTime;
            transform.up = direction;
            yield return null;
        }
    }

    void DestoryProjectile()
    {
        Destroy(gameObject);
    }

    void CancelFire()
    {
        Destroy(gameObject);
        Debug.LogWarning("Arrow fired without being initlized.");
    }

    //IEnumerator Die(float delay)
    //{
    //    _speed = 0;
    //    yield return new WaitForSeconds(delay);
    //    Destroy(gameObject);
    //}

    //IEnumerator TrackPositions(bool destroyAfterTargets, float deathDelay)
    //{
    //    yield return null;
    //    Vector2 direction = (_worldTargets[0] - (Vector2)transform.position).normalized;
    //    Vector2 prevDirection = direction;
    //    bool hitTarget = false;
    //    for (int i = 0; i < _worldTargets.Length; i++)
    //    {

    //        while (_worldTargets[i] != null && (Vector2.Distance(transform.position, _worldTargets[i]) > HIT_TOLLERANCE || hitTarget == false))
    //        {
    //            if (Vector2.Dot(direction.normalized, prevDirection.normalized) <= 0)
    //            {
    //                hitTarget = true;
    //                direction = prevDirection;
    //                break;
    //            }

    //            prevDirection = direction;
    //            direction = (_worldTargets[i] - (Vector2)transform.position).normalized;
    //            transform.position += (Vector3)direction * _speed * Time.deltaTime;
    //            transform.up = direction;
    //            yield return null;
    //        }
    //        direction = (_worldTargets[i] - (Vector2)transform.position).normalized;
    //        hitTarget = (_worldTargets[i] != null || Vector2.Dot(direction.normalized, prevDirection.normalized) <= 0);
    //    }
    //    if (hitTarget && destroyAfterTargets)
    //        StartCoroutine(Die(deathDelay));
    //    //Destroy(gameObject);
    //    else
    //    {
    //        transform.up = _orginalDirection;
    //        while (true)
    //        {
    //            transform.position += (Vector3)_orginalDirection * _speed * Time.deltaTime;
    //            yield return null;
    //        }
    //    }
    //}

    //private void OnDestroy()
    //{
    //    finished = true;
    //}

    //IEnumerator TrackTargets(bool destroyAfterTargets)
    //{
    //    yield return null;
    //    if (_targetsT[0] == null)
    //    {
    //        StartCoroutine(Die(0));
    //        yield break;
    //    }
    //    Vector2 direction = (_targetsT[0].position - transform.position).normalized;
    //    Vector2 prevDirection = direction;
    //    bool hitTarget = false;
    //    for (int i = 0; i < _targetsT.Length; i++)
    //    {

    //        while (_targetsT[i] != null && (Vector2.Distance(transform.position, _targetsT[i].position) > HIT_TOLLERANCE || hitTarget == false))
    //        {
    //            direction = (_targetsT[i].position - transform.position).normalized;

    //            if (Vector2.Dot(direction.normalized, prevDirection.normalized) <= 0)
    //            {
    //                hitTarget = true;
    //                direction = prevDirection;
    //            }

    //            transform.position += (Vector3)direction * _speed * Time.deltaTime;
    //            transform.up = direction;
    //            prevDirection = direction;
    //            yield return null;
    //        }
    //        hitTarget = true;
    //        prevDirection = direction;
    //    }
    //    if (hitTarget && destroyAfterTargets)
    //    {

    //        finished = true;
    //        if (_targetsT[_targetsT.Length - 1] == null)
    //        {
    //            StartCoroutine(JustGo(direction));
    //            yield break;
    //        }
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        transform.up = _orginalDirection;
    //        while (true)
    //        {
    //            if (returnToSource)
    //            {
    //                transform.position += ((Vector3)_origin - transform.position).normalized * _speed * Time.deltaTime;
    //                if (Vector2.Distance(transform.position, _origin) < HIT_TOLLERANCE)
    //                {
    //                    finished = true;
    //                    yield return null;
    //                    Destroy(gameObject);
    //                }
    //            }
    //            else
    //            {
    //                transform.position += (Vector3)_orginalDirection * _speed * Time.deltaTime;
    //            }
    //            yield return null;
    //        }
    //    }
    //}

    ///// <summary>
    ///// Summon a projectile using dynamic Transforms as targets.
    ///// </summary>
    ///// <param name="projectile"></param>
    ///// <param name="spawnPosition"></param>
    ///// <param name="targets"></param>
    ///// <param name="aoe"></param>
    ///// <param name="speed"></param>
    ///// <returns></returns>
    //public static Projectile CreateNewProjectile(GameObject projectile, Vector3 spawnPosition, Transform[] targets, float[] aoe, float speed, bool destroyAfterTargets = true)
    //{
    //    //check if we're firing at anything
    //    if (targets.Length == 0)
    //    {
    //        return null;
    //    }

    //    //create the projectile.
    //    GameObject go = GameObject.Instantiate(projectile, spawnPosition, Quaternion.identity);
    //    Projectile proj = go.GetComponent<Projectile>();

    //    //proj._origin = spawnPosition;
    //    proj._speed = speed;
    //    proj._origin = spawnPosition;
    //    proj._orginalDirection = (targets[0].position - spawnPosition).normalized;
    //    proj._targetsT = targets;
    //    proj._initilized = true;

    //    proj.StartCoroutine(proj.TrackTargets(destroyAfterTargets));
    //    return proj;
    //}

    ///// <summary>
    ///// Summon a projectile using fixed world positions as targets.
    ///// </summary>
    ///// <param name="projectile"></param>
    ///// <param name="spawnPosition"></param>
    ///// <param name="targets"></param>
    ///// <param name="aoe"></param>
    ///// <param name="speed"></param>
    ///// <returns></returns>
    //public static Projectile CreateNewProjectile(GameObject projectile, Vector3 spawnPosition, Vector3[] targets, float[] aoe, float speed, bool destroyAfterTargets = true, float deathDelay = 0f)
    //{
    //    //check if we're firing at anything
    //    if (targets.Length == 0)
    //    {
    //        return null;
    //    }

    //    //create the projectile.
    //    GameObject go = GameObject.Instantiate(projectile, spawnPosition, Quaternion.identity);
    //    Projectile proj = go.GetComponent<Projectile>();

    //    Vector2[] targets2d = new Vector2[targets.Length];

    //    for (int i = 0; i < targets.Length; i++)
    //    {
    //        targets2d[i] = targets[i];
    //    }

    //    //proj._origin = spawnPosition;
    //    proj._speed = speed;
    //    proj._origin = spawnPosition;
    //    proj._orginalDirection = (targets[0] - spawnPosition).normalized;
    //    proj._targetsP = targets;
    //    proj._worldTargets = targets2d;
    //    proj._initilized = true;

    //    proj.StartCoroutine(proj.TrackPositions(destroyAfterTargets, deathDelay));
    //    return proj;
    //}
}
