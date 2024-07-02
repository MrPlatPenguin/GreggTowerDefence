using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RomanNumerals : MonoBehaviour
{
    [SerializeField] Sprite[] numberSprites;
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer spriteRenderer;


    public void ShowNumber(int number)
    {
        if (image != null)
        {
            image.sprite = numberSprites[number];
            image.SetNativeSize();
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sprite = numberSprites[number];
        }
    }
}
