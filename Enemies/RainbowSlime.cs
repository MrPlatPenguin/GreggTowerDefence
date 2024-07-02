using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowSlime : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _speed = 10;
    float _hue, _saturation, _value;

    private void Awake()
    {
        Color.RGBToHSV(_spriteRenderer.color, out _hue, out _saturation, out _value);
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.color = Color.HSVToRGB(_hue / 360f, _saturation, _value);
        _hue = (_hue + Time.deltaTime * _speed) % 360;
    }
}
