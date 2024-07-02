using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundClutter : MonoBehaviour
{
    [SerializeField] SpriteRenderer _clutterGO;
    [SerializeField] int _ammountOfClutter;
    [SerializeField] ClutterArray[] clutters;
    [SerializeField] Sprite sprite;

    public void SpreadGroundClutter()
    {
        if (!OptionsSave.Save.ShowGroundClutter)
            return;

            for (int i = 0; i < _ammountOfClutter; i++)
        {
            Vector2 pos = (Random.insideUnitCircle * MapGenerator.MapRadius * MapGenerator.CellSize) + MapGenerator.MapCentre;
            Sprite[] sprites = clutters[MapGenerator.BiomeNoise.GetValue(pos)]._sprites;

            if (sprites.Length <= 0)
                continue;
            Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];


            SpriteRenderer spriteRenderer = Instantiate(_clutterGO, pos, Quaternion.identity, transform);
            spriteRenderer.sprite = randomSprite;

        }
    }
}

[System.Serializable]
public class ClutterArray
{
    [SerializeField] string name;
    public Sprite[] _sprites;
}
