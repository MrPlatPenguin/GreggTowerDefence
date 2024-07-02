using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoader : MonoBehaviour
{
    public static bool NewGame = true;
    [SerializeField] GameSaveAndLoad gameSaver;
    [SerializeField] UnityEvent onLoadEventExecutions;

    private void Awake()
    {
        if (NewGame)
            gameSaver.CreateNewSave();
        else
            gameSaver.LoadSave();

        onLoadEventExecutions?.Invoke();
    }
}
