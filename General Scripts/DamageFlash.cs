using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _spriteRenderer;
    public Color[] startColour;

    private void Awake()
    {
        SetStartColors();
    }

    public void SetStartColors()
    {
        startColour = new Color[_spriteRenderer.Length];

        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            startColour[i] = _spriteRenderer[i].color;

        }
    }

    public void CancelFlash()
    {
        StopAllCoroutines();
        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            _spriteRenderer[i].material.SetFloat("_Alpha", 0);
        }
    }

    public void Flash(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash(duration));
    }

    IEnumerator DoFlash(float duration)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError(gameObject.name + " damage flash is missing the reference to its sprite render.");
        }

        float t = 0f;
        float alpha;

        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            _spriteRenderer[i].material.SetFloat("_Alpha", 1);
        }

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            alpha = Mathf.Lerp(2f, 0f, t);
            for (int i = 0; i < _spriteRenderer.Length; i++)
            {
                _spriteRenderer[i].material.SetFloat("_Alpha", alpha);
                _spriteRenderer[i].color = Color.Lerp(Color.white, startColour[i], t);
            }
            yield return null;
        }

        // Set alpha to 0 to ensure it ends at 0.
        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            _spriteRenderer[i].material.SetFloat("_Alpha", 0);

        }
    }

}
