using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _dayMusic;
    [SerializeField] AudioClip _nightMusic;

    [SerializeField] SoundClip _pause, _play;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.OnDayChange += () => SetMusic(_dayMusic);
        GameManager.OnNightStart += () => SetMusic(_nightMusic);
        SetMusic(_dayMusic);

        GameManager.OnGamePause += () => _source.Pause();
        GameManager.OnGameResume += () => _source.UnPause();

        GameManager.OnGamePause += () => SoundManager.PlaySound(_pause, CameraController.instance.transform);
        GameManager.OnGameResume += () => SoundManager.PlaySound(_play, CameraController.instance.transform);
    }

    void SetMusic(AudioClip clip)
    {
        _source.clip = clip;
        _source.Play();
    }
}
