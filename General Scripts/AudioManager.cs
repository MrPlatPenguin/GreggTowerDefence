using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    [SerializeField] AudioMixerGroup masterGroup, musicGroup, sfxGroup;
    static AudioMixerGroup MasterGroup { get => instance.masterGroup; }
    static AudioMixerGroup MusicGroup { get => instance.musicGroup; }
    static AudioMixerGroup SFXGroup { get => instance.sfxGroup; }

    List<AudioSource> audioSourcePool = new List<AudioSource>();


    private void Awake()
    {
        instance = this;
    }

    public static void Apply(SoundManager.SoundType soundType, AudioSource source)
    {
        switch (soundType)
        {
            case SoundManager.SoundType.Default:
                source.outputAudioMixerGroup = MasterGroup;
                break;
            case SoundManager.SoundType.Music:
                source.outputAudioMixerGroup = MusicGroup;
                break;
            case SoundManager.SoundType.SFX:
                source.outputAudioMixerGroup = SFXGroup;
                break;
            default:
                break;
        }
    }

    public static AudioSource GetAvailableAudioSource()
    {
        AudioSource source = instance.audioSourcePool.Find(s => !s.isPlaying);
        if (source == null)
        {
            GameObject go = new GameObject("AudioSource", typeof(AudioSource));
            source = go.GetComponent<AudioSource>();
            instance.audioSourcePool.Add(source);
        }

        return source;
    }

    public static void ClearAudioPool()
    {
        instance.audioSourcePool.Clear();
    }
}
