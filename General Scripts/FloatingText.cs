using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FloatingText : MonoBehaviour
{
    System.DateTime _timeCreated;
    Vector2 _spawnPoint;
    Vector2 _endPoint;
    float _duration;
    TextMeshPro _text;

    public static TextMeshPro Create(string text, Vector3 position, Color textColor, float duration = 1f, float distance = 0.5f, float fontSize = 10f)
    {
        GameObject floatingTextGO = new GameObject("Floating Text");
        FloatingText floatingText = floatingTextGO.AddComponent<FloatingText>();
        floatingText._spawnPoint = position;
        floatingText._endPoint = floatingText._spawnPoint + (Vector2.up * distance * MapGenerator.CellSize);
        floatingText._timeCreated = System.DateTime.Now;
        floatingText._duration = duration;
        new Delay(floatingText, duration, () => Destroy(floatingText.gameObject));

        SortingGroup sortingGroup = floatingTextGO.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");

        TextMeshPro textMesh = floatingTextGO.AddComponent<TextMeshPro>();
        floatingText._text = textMesh;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = textColor;
        floatingTextGO.transform.position = position;
        floatingText.StartCoroutine(floatingText.RunLerp());
        return textMesh;
    }

    IEnumerator RunLerp()
    {
        while(true)
        {
            //runTime = Mathf.Clamp(runTime + 0.001f, 0, _duration);
            //float t = runTime / _duration;
            float t = (float)(System.DateTime.Now - _timeCreated).TotalSeconds / _duration;
            transform.localScale = Vector3.one * Mathf.Lerp(1, 0.75f, t);
            transform.position = Vector2.Lerp(_spawnPoint, _endPoint, t);

            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1f - t);
            yield return null;
        }
    }
}
