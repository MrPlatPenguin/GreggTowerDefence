using System;
using UnityEngine;

public interface IDamageReciver
{
    public void TakeDamage(float damage, Enemy attacker = null);
}
