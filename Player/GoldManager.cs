using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager PlayerGold;
    [SerializeField] int _gold;
    public event Action OnChangeValue;

    private void Awake()
    {
        if (gameObject.CompareTag("Player"))
        {
            PlayerGold = this;
        }
    }

    public bool CanAfford(int gold)
    {
        return gold <= _gold;
    }

    public void Spend(int gold)
    {
        _gold = Mathf.Clamp(_gold - gold, 0, int.MaxValue);
        OnChangeValue?.Invoke();
    }

    public void AddGold(int gold)
    {
        _gold = Mathf.Clamp(_gold + gold, 0, int.MaxValue);
        OnChangeValue?.Invoke();
    }

    public int GetGold()
    {
        return _gold;
    }

    public int SetGold(int gold)
    {
        _gold = gold;
        OnChangeValue?.Invoke();
        return _gold;
    }
}
