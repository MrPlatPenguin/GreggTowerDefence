using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class SoundManager 
{
    public enum SoundType
    {
        Music,
        SFX,
        Default
    }
    public static AudioMixerGroup mixer;

    public static void PlaySound(SoundClip soundClip, Transform parent, bool attatchToParent = true)
    {
        if (soundClip == null || IsNotOnScreen(parent.position))
            return;

        AudioSource source = AudioManager.GetAvailableAudioSource();
        GameObject go = source.gameObject;

        go.transform.position = parent.transform.position;

        source.volume = soundClip.volume;
        source.spatialBlend = soundClip.is3D ? 1f : 0f;
        source.pitch = 1 + Random.Range(-soundClip.pitchShift, soundClip.pitchShift);
        source.PlayOneShot(soundClip.clip);

        AudioManager.Apply(soundClip.soundType, source);
    }

    static bool IsNotOnScreen(Vector2 location)
    {
        // Convert the location from world space to screen space
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(location);

        // Check if the screen point is within the screen bounds
        return !(screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height);
    }


}