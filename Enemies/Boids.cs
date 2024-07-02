using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boids : MonoBehaviour
{
    [SerializeField] float _sightRange = 1;
    float sightRange { get { return _sightRange * MapGenerator.CellSize; } }
    float _maxSpeed;
    float maxSpeed { get { return _maxSpeed * MapGenerator.CellSize; } }
    public Vector2 Velocity { get; private set; }
    [SerializeField] float turningSpeed;
    [SerializeField] float _slowingRadius;
    float slowingRadius { get { return _slowingRadius * MapGenerator.CellSize; } }
    float stoppingDistance { get { return 0.75f * MapGenerator.CellSize; } }
    [SerializeField] float avoidanceForce;
    Vector3 target;
    public Rigidbody2D rb { get; private set; }

    [SerializeField] float avoidanceRange;

    [SerializeField] CircleCollider2D avoidanceCircle;

    bool _disabled;

    [SerializeField] SpriteAnimator _spriteAnim; 

    Transform _playerTransform;

    Vector3 _towerTransform;

    public bool IsGrounded { get; private set; } = true;

    [Header("SFX")]
    [SerializeField] SoundClip jumpSound;

    bool _facingRight;

    private void Awake()
    {
        _playerTransform = Player.Transform;
        _towerTransform = MapGenerator.MapCentre;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_disabled || _spriteAnim == null)
            return;

        if (IsGrounded)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        //Vector2 steering = Vector2.zero;
        //steering += Seek(target);

        Vector2 direction = target - transform.position;
        float distance = direction.magnitude;
        float speedScaler = Mathf.Clamp01((distance - stoppingDistance) / slowingRadius);
        Velocity = Vector2.ClampMagnitude(direction, maxSpeed);
        //if (speedScaler > 0)
        //    steering += Avoidance() * Mathf.Clamp01(speedScaler);

        //steering = Vector2.ClampMagnitude(steering, turningSpeed);
        //Velocity = Vector2.ClampMagnitude(Velocity + steering, maxSpeed);
        //transform.Translate(Velocity * Time.deltaTime);

        rb.velocity = Velocity * speedScaler;

        OrientSprite();

        SetTargetingPlayer(false);
    }

    private void LookForPlayer()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < sightRange)
            target = _playerTransform.position;
        else
            target = _towerTransform != null ? _towerTransform : CharacterController.Instance.transform.position;
    }

    public void SetTargetingPlayer(bool value)
    {
        if (value)
            target = _playerTransform.position;
        else
            target = _towerTransform != null ? _towerTransform : CharacterController.Instance.transform.position;
    }

    Vector2 Seek(Vector2 target)
    {
        Vector2 desiredVelocity = target - (Vector2)transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - Velocity;
    }

    Vector2 Flee(Vector2 target)
    {
        Vector2 desiredVelocity = (Vector2)transform.position - target;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - Velocity;
    }

    Vector2 Avoidance()
    {
        Vector2 avoidance = Vector2.zero;
        Vector2 ahead = (Vector2)transform.position + (Velocity.normalized * avoidanceRange);

        Transform closest = null;
        float closetDistance = Mathf.Infinity;

        Enemy[] obstacles = Enemy.GetEnemyQuadTree().EnemiesInRadius(transform.position, avoidanceRange);

        foreach (Enemy obstacle in obstacles)
        {
            if (Vector2.Distance(obstacle.transform.position, transform.position) < closetDistance)
            {
                closest = obstacle.transform;
            }
        }

        if (closest == null)
            return Vector2.zero;

        avoidance.x = ahead.x - closest.position.x;
        avoidance.y = ahead.y - closest.position.y;

        return avoidance.normalized * avoidanceForce;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        boidsInRange.Add(collision.transform);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        boidsInRange.Remove(collision.transform);
    //    }
    //}

    public void Disable(float durtation)
    {
        if (!gameObject.activeSelf)
            return;
        StartCoroutine(DisableTimer(Time.time + durtation));
    }

    IEnumerator DisableTimer(float endTime)
    {
        _disabled = true;
        while (Time.time < endTime)
        {
            yield return null;
        }
        _disabled = false;
    }

    public void Disable()
    {
        _disabled = true;
    }

    public void Enable()
    {
        _disabled = false;

    }

    public void SetSightDistance(float newDistance)
    {
        _sightRange = newDistance;
    }

    public void SetGrounded(bool value)
    {
        IsGrounded = value;
    }

    public void PlayJumpSound()
    {
        SoundManager.PlaySound(jumpSound, transform, false);
    }

    void OrientSprite()
    {
        // Only execute the flip if the distance is greater than the threshold
        Vector2 direction = Velocity.x > 0 ? Vector2.left : Vector2.right;

        if (_facingRight != (direction == Vector2.right))
        {
            // Flip the object by changing its local scale
            _spriteAnim.transform.localScale = new Vector3(-_spriteAnim.transform.localScale.x, _spriteAnim.transform.localScale.y, _spriteAnim.transform.localScale.z);
            _facingRight = direction == Vector2.right;
        }
    }

    public void SetSpeed(float speed)
    {
        _maxSpeed = speed;
    }

    public void ResetAI()
    {
        StopAllCoroutines();
        IsGrounded = true;
        Enable();
    }
}