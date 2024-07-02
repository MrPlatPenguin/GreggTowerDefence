using UnityEngine;

public class MouseOverHighlight : MonoBehaviour
{
    SpriteRenderer[] spriteRenderers;
    Color[] _defaultColors;
    Color _highlightColour = Color.green;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _defaultColors = new Color[spriteRenderers.Length];
    }

    private void OnMouseEnter()
    {
        int index = 0;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            _defaultColors[index] = spriteRenderer.color;
            spriteRenderer.color = new Color(_highlightColour.r, _highlightColour.g, _highlightColour.b, spriteRenderer.color.a);
            index++;
        }
    }

    private void OnMouseExit()
    {
        int index = 0;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = _defaultColors[index];
            index++;
        }
    }
}
