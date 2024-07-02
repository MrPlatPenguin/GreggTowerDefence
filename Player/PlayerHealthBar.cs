using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Material healthSlider;

    private void Awake()
    {
        //healthSlider = GetComponent<Slider>();
        PlayerEvents.OnHealthChange += UpdateHealthBar;
        //StartCoroutine(LerpWaveStrength(10, 40));

    }

    void UpdateHealthBar(float health, float change)
    {
        //healthSlider.value = Mathf.Clamp01(health / Player.MaxHealth);
        StartCoroutine(LerpWaveHeight(0.2f, healthSlider.GetFloat("_Water_Height"), Mathf.Clamp01(health / Player.MaxHealth) * 0.52f));
        healthSlider.SetFloat("_Water_Height", Mathf.Clamp01(health / Player.MaxHealth) * 0.52f);
        StartCoroutine(LerpWaveStrength(0.8f, Random.Range(12, 20)));
        //healthSlider.SetFloat("_Water_Height", 0.5f);
    }

    IEnumerator LerpWaveHeight(float duration, float start, float end)
    {
        float timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            healthSlider.SetFloat("_Water_Height", Mathf.Lerp(start, end, timeElapsed / duration));

            yield return null;
        }
    }

    IEnumerator LerpWaveStrength(float duration, float maxStrength)
    {
        float timeElapsed = 0;
        float nTimeElapsed = 0;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            nTimeElapsed = (timeElapsed / duration)*2;
            healthSlider.SetFloat("_Wave_Strength", Mathf.Lerp(3, maxStrength, nTimeElapsed < 1 ? nTimeElapsed : 1 - (nTimeElapsed - 1)));
            float test = nTimeElapsed < 1 ? nTimeElapsed : 1 - (nTimeElapsed - 1);
            yield return null;
        }
    }
}
