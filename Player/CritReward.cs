using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crit Reward", menuName = "CritReward")]
public class CritReward : ScriptableObject
{
    public int minResources;
    public int maxResources;
    public float damageMultiplier = 1f;
    public Color color;
    public bool Success;
}