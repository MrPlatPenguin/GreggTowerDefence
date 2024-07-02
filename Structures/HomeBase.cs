using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : Structure
{
    public static HomeBase Instance;
    bool inBase = false;
    bool justEntered = false;
    [SerializeField] Transform _playerSpawn;
    Transform _playerTransform;
    [SerializeField] GameObject _arrowGO;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        GameManager.OnFirstNight += ()=> TutorialArrow();
    }

    protected override void Update()
    {
        base.Update();

        bool moveInputDetected = MoveInputDetected();

        // When player lets go of movement sets this to false
        justEntered = justEntered && moveInputDetected;
        if (inBase && !GameManager.IsGamePaused)
        {
            if (!justEntered && moveInputDetected)
            {
                Exit();
            }
        }
    }

    void TutorialArrow()
    {
        if (!inBase)
            _arrowGO.SetActive(true);
    }

    protected override void RunAttackSquence()
    {
    }

    void Enter()
    {
        PlayerEvents.OnEnterBase();
        PlayFireSound();
        _arrowGO.SetActive(false);
        justEntered = true;
        inBase = true;
    }

    void Exit()
    {
        PlayFireSound();
        _playerTransform.position = _playerSpawn.position;
        PlayerEvents.OnExitBase?.Invoke();
        inBase = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            _playerTransform = collision.transform;
            Enter();
        }
    }

    protected override Enemy TryGetAttackTarget()
    {
        return null;
    }

    public override void TakeDamage(float damage, Enemy attacker)
    {
        base.TakeDamage(damage, attacker);
        if (CurrentHealth <= 0)
            GameManager.GameOver("Your base was destroyed");
    }

    bool MoveInputDetected()
    {
        return (Input.GetAxisRaw("Vertical") != 0) ||
            (Input.GetAxisRaw("Horizontal") != 0);
    }

    protected override float GetProjectileSpeed()
    {
        throw new System.NotImplementedException();
    }

    
}