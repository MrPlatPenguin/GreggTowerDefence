using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation", menuName = "Animation")] 
public class SpriteAnimation : ScriptableObject
{
    public Sprite[] Frames;

    public int FPS = 12;
    public float FrameRate { get => 1f / FPS; }

    public bool Loop = true;

    public bool unscaledTime = false;

    public event Action OnAnimationEnter;
    public event Action OnAnimationUpdate;
    public event Action OnAnimationExit;
    public event Action OnAnimationChange;

    public virtual void Enter() => OnAnimationEnter?.Invoke();
    public virtual void Update() => OnAnimationUpdate?.Invoke();
    public virtual void Exit() => OnAnimationExit?.Invoke();
}