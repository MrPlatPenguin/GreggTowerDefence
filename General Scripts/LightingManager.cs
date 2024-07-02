using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    [SerializeField] Light2D _globalLight;
    [SerializeField] AnimationCurve _dayLightCurve;

    Light2D[] _lights;
    float[] intesities;

    public void GetAllLights()
    {
        List<Light2D> lights = FindObjectsOfType<Light2D>().ToList();
        lights.Remove(_globalLight);
        _lights = lights.ToArray();
        intesities = new float[_lights.Length];
        for (int i = 0; i < _lights.Length; i++)
        {
            intesities[i] = _lights[i].intensity;
        }
    }

    private void Update()
    {
        float dayPerc = Mathf.Clamp01(GameManager.TimeSinceDayStart / GameManager.DayLength);
        _globalLight.intensity = _dayLightCurve.Evaluate(dayPerc);
        for (int i = 0; i < _lights.Length; i++)
        {
            _lights[i].intensity = (1 - _dayLightCurve.Evaluate(dayPerc)) * intesities[i];
        }
    }
}
