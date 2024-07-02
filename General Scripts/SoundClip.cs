using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Clip", menuName = "Sound Clip")]
public class SoundClip : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1f;
    public float pitchShift = 0f;
    public SoundManager.SoundType soundType;
    public bool is3D;
}
