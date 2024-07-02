using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] TextMeshProUGUI _distanceText;

    Transform playerTransform;
    Vector2 mapCentre;
    float cellSize;
    int homeSize;

    bool initialized;

    private void Awake()
    {
        playerTransform = Player.Transform;
        mapCentre = MapGenerator.MapCentre;
        cellSize = MapGenerator.CellSize;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
            return;

        Vector2 direction = mapCentre - (Vector2)playerTransform.position;
        int distance = Mathf.RoundToInt((direction.magnitude / cellSize) - MapGenerator.HomeSize);

        transform.up = direction.normalized;
        _distanceText.text = distance.ToString();
        _distanceText.transform.up = Vector2.up;
        Show(CharacterController.Instance.GetCurrentBiome().Name != "Home");
    }

    void Init(Player player)
    {

    }


    void Show(bool show)
    {
        _sprite.gameObject.SetActive(show);
        _distanceText.gameObject.SetActive(show);
    }
}