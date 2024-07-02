using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    Rigidbody2D rb2D;
    [SerializeField] ShoesSO[] _shoes;
    [SerializeField] int _shoesIndex;
    public int ShoesUpgradeCost { get => _shoesIndex < _shoes.Length -1 ? _shoes[_shoesIndex + 1].Cost : -1; }
    public static ShoesSO CurrentShoes { get => Instance._shoes[Instance._shoesIndex]; }
    Vector2 inputDirection;
    Vector2 currentVelocity;
    public static CharacterController Instance;
    [SerializeField] Collider2D _collider;
    bool isAttackng;
    float attackCooldown;
    public bool isMoving { get; private set; }

    [Header("Animation")]
    [SerializeField] Animator _anim;
    string xHash { get => "X"; }
    string yHash { get => "Y"; }

    private void Awake()
    {
        Instance = this;
        rb2D = GetComponent<Rigidbody2D>();

        PlayerEvents.OnEnterBase += EnterBase;
        PlayerEvents.OnExitBase += ExitBase;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input for horizontal and vertical movement
        float moveX = 0;
        float moveY = 0;

        if (!Player.InBase)
        {
            if (Input.GetKey(KeyCode.W))
                moveY++;
            if (Input.GetKey(KeyCode.S))
                moveY--;
            if (Input.GetKey(KeyCode.A))
                moveX--;
            if (Input.GetKey(KeyCode.D))
                moveX++;
        }

        // Normalize the move direction to ensure consistent movement speed
        inputDirection = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        float attackingScaler = isAttackng ? 0.5f : 1f;
        // Update the character's velocity based on the move direction and acceleration
        if (inputDirection != Vector2.zero)
            currentVelocity = Vector2.MoveTowards(currentVelocity, inputDirection * attackingScaler * CurrentShoes.Speed, CurrentShoes.Acceleration * Time.deltaTime);

        // If the character is not moving, apply deceleration to bring them to a stop
        else
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, CurrentShoes.Deceleration * Time.deltaTime);

        rb2D.velocity = currentVelocity;
        isMoving = rb2D.velocity.magnitude > 0f;

        // Update animator
        _anim.SetFloat(xHash, rb2D.velocity.normalized.x);
        _anim.SetFloat(yHash, rb2D.velocity.normalized.y);
    }

    public void SetIsAttacking(float time)
    {
        _anim.speed = 0.5f;
        bool onCooldown = attackCooldown > 0;
        attackCooldown = time * 1.01f;
        isAttackng = true;

        if (!onCooldown)
            StartCoroutine(ResetAttackValue());
    }

    IEnumerator ResetAttackValue()
    {
        while (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            yield return null;
        }
        isAttackng = false;
        attackCooldown = 0f;
        _anim.speed = 1f;
    }

    public Biome GetCurrentBiome()
    {
        MapGenerator.Grid.GetXY((Vector2)transform.position + _collider.offset, out int x, out int y);
        return MapGenerator.Grid.GetGridObject(x, y).biome;
    }

    void EnterBase()
    {
        transform.position= Vector2.one * 10f;
    }

    void ExitBase()
    {

    }

    public int UpgradeShoes()
    {
        GoldManager.PlayerGold.Spend(ShoesUpgradeCost);
        _shoesIndex++;
        PlayerEvents.OnUpgradeShoes?.Invoke();
        return _shoesIndex;
    }

    public bool CanAffordShoesUpgrade()
    {
        if (_shoesIndex >= _shoes.Length - 1)
            return false;

        return ShoesUpgradeCost <= GoldManager.PlayerGold.GetGold();
    }

    public static void SetShoeLevel(int value)
    {
        Instance._shoesIndex = value;
        PlayerEvents.OnUpgradeShoes?.Invoke();
    }

    public static int GetShoeLevel() { return Instance._shoesIndex; }

}