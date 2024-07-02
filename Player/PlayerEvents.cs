using System;

public static class PlayerEvents
{
    public delegate void PlayerDelegate(Player player);
    public static PlayerDelegate OnPlayerSpawn;

    public static Action OnTakeDamage;

    public static Action OnEnterBase;

    public static Action OnExitBase;

    public delegate void HealthChange(float health, float change);
    public static HealthChange OnHealthChange;

    public static Action OnUpgradeAxe;
    public static Action OnUpgradeShoes;


    public static void ResetEvents()
    {
        OnPlayerSpawn = null;
        OnTakeDamage = null;
        OnEnterBase = null;
        OnExitBase = null;
        OnHealthChange = null;
        OnUpgradeAxe = null;
        OnUpgradeShoes = null;
    }
}
