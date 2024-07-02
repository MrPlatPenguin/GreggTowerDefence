using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatKeys : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            int num = 1000;
            ResourceManager.playerResources.Add(new ResourceManager(num, num, num, num, num));
            GoldManager.PlayerGold.AddGold(1000);

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            int num = 0;
            ResourceManager.playerResources.Set(num, num, num, num, num);
            GoldManager.PlayerGold.Spend(GoldManager.PlayerGold.GetGold());

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameManager.SkipDay();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            GameManager.SkipToNight();
        }
    }
}
