using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string _gameScene;
    [SerializeField] GameObject _creditsMenu;

    [SerializeField] GameObject _playBtn;
    [SerializeField] GameObject _continueBtns;

    private void Awake()
    {
        PlayerEvents.ResetEvents();
        StructureEvents.ResetEvents();
        GameSave gameSave = GameSaveAndLoad.TryGetSave();
        if (gameSave == null || gameSave.day <= 1)
        {
            _playBtn.SetActive(true);
            _continueBtns.SetActive(false);
        }
        else
        {
            _playBtn.SetActive(false);
            _continueBtns.SetActive(true);
        }
        Time.timeScale = 1.0f;
    }

    public void PlayGame(bool newGame)
    {
        GameLoader.NewGame = newGame;
        SceneManager.LoadScene(_gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleCreditsMenu()
    {
        _creditsMenu.SetActive(!_creditsMenu.activeInHierarchy);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
