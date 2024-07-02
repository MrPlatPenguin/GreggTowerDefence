using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SpriteAnimator : MonoBehaviour
{
    // TODO: Move events from the sprite animation script to here so that it only calls the events locally.
    // TODO: Animations that don't loop but have finished animating should freeze on the last frame.
    [SerializeField] int defaultAnimationState = 0;
    [FormerlySerializedAs("animationHolderColl")]
    [FormerlySerializedAs("animations")]
    [SerializeField] AnimationState[] animationStates = new AnimationState[0];

    private SpriteAnimation currentAnimation;
    int currentAnimationState;

    int _currentFrame;
    float _timer;

    SpriteRenderer spriteRenderer;

    List<SpriteAnimation> _queuedAnimation = new List<SpriteAnimation>();

    bool paused;

    private void Awake()
    {
        foreach (AnimationState animHolder in animationStates)
        {
            animHolder.name = animHolder.animation.name;
        }

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ResetAnimator();
    }

    void Update()
    {
        if (currentAnimation == null)
            return;

        currentAnimation.Update();

        if (paused)
            return;

        _timer += currentAnimation.unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (_timer >= currentAnimation.FrameRate)
            NextFrame();

    }

    void NextFrame()
    {
        _timer -= currentAnimation.FrameRate;
        _currentFrame++;

        if (_currentFrame == currentAnimation.Frames.Length)
        {
            NextAnimationInQueue();
            return;
        }

        SetFrame(_currentFrame);
    }

    void SetFrame(int index)
    {
        _currentFrame = index;
        spriteRenderer.sprite = currentAnimation.Frames[index];
        InvokeFrameEvents();
    }

    void NextAnimationInQueue()
    {
        if (_queuedAnimation.Count > 0)
        {
            SetAnimation(_queuedAnimation[0]);
            _queuedAnimation.RemoveAt(0);
            return;
        }

        if (currentAnimation.Loop)
        {
            SetAnimation(currentAnimationState);
            return;
        }

        paused = true;
    }

    /// <summary>
    /// Immediately change to the indexed function.
    /// </summary>
    /// <param name="animationName"></param>
    public void SetAnimation(int index)
    {
        currentAnimation.Exit();
        _currentFrame = 0;
        _timer = 0;

        currentAnimation = animationStates[index].animation;
        currentAnimationState = index;
        SetFrame(_currentFrame);

        currentAnimation.Enter();
        paused = false;
    }

    /// <summary>
    /// Immediately change to the referenced function.
    /// </summary>
    /// <param name="animationName"></param>
    public void SetAnimation(SpriteAnimation animation)
    {
        for (int i = 0; i < animationStates.Length; i++)
        {
            if (animationStates[i].animation == animation)
            {
                SetAnimation(i);
                return;
            }
        }
        // This point is only reached if the animation is not in the array.
        AddAnimation(animation);
    }

    /// <summary>
    /// Immediately change to the named function.
    /// </summary>
    /// <param name="animationName"></param>
    public void SetAnimation(string name)
    {
        SetAnimation(GetIndexFromAnimationName(name));
    }

    void InvokeFrameEvents()
    {
        foreach (AnimationEventHandler animEvent in animationStates[currentAnimationState].events)
        {
            if (animEvent.frame == _currentFrame)
            {
                animEvent.animEvent?.Invoke();
            }
        }
    }

    void AddAnimation(SpriteAnimation animation)
    {
        List<AnimationState> newAnimations = new List<AnimationState>();
        newAnimations.AddRange(animationStates);
        AnimationState newAnimHolder = new AnimationState();
        newAnimHolder.animation = animation;
        newAnimHolder.name = animation.name;
        newAnimations.Add(newAnimHolder);
        animationStates = newAnimations.ToArray();
    }

    void OldUpdate()
    {
        if (currentAnimation == null)
            return;

        if (_currentFrame == 0)
        {
            //Event sent out to subscribed methods.
            currentAnimation.Enter();
        }

        //Count how much time has gone by, compare that to the frame rate,
        //and increment frames if we've waited long enough.

        _timer += GetCurrentAnimation().unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (_timer >= currentAnimation.FrameRate)
        {

            AnimationEventHandler[] animEvents = animationStates[GetIndexFromAnimationName(currentAnimation.name)].events;

            foreach (AnimationEventHandler animEvent in animEvents)
            {
                if (animEvent.frame == _currentFrame)
                {
                    animEvent.animEvent?.Invoke();
                }
            }

            _timer -= currentAnimation.FrameRate;
            _currentFrame = (_currentFrame + 1) % currentAnimation.Frames.Length;
            //If the animation has finished and we have an animation queued, play that.
            if (_currentFrame == 0 && _queuedAnimation.Count > 0)
            {
                currentAnimation.Exit();
                //animation.Change();
                currentAnimation = _queuedAnimation[0];
                _queuedAnimation.RemoveAt(0);
                spriteRenderer.sprite = currentAnimation.Frames[0];
                return;
            }
            //otherwise if the animation does not loop, exit the animation.
            else if (_currentFrame == 0 && !currentAnimation.Loop)
            {
                //Event sent out to subscribed methods.
                currentAnimation.Exit();
                currentAnimation = animationStates[defaultAnimationState].animation;
                return;
            }

            //check to see if an event was triggered.

            spriteRenderer.sprite = currentAnimation.Frames[_currentFrame];

            //foreach (AnimationEventHandler animationEvent in ) //this is wrong change this.
            //{
            //    if (animationEvent.frame == _currentFrame)
            //    {
            //        animationEvent.myEvent?.Invoke();
            //    }
            //}
        }
        //Event sent out to subscribed methods.
        currentAnimation.Update();
    }

    public SpriteRenderer GetSpriteRenederer()
    {
        return spriteRenderer;
    }

    private int GetIndexFromAnimationName(string animationName)
    {
        foreach (AnimationState animHold in animationStates)
        {
            if (animationName == animHold.name)
            {
                return Array.IndexOf(animationStates, animHold);
            }
        }

        return -1;
    }

    /// <summary>
    /// Queue an animation to be played after the current (and all prior queued) animation(s) have finished.
    /// </summary>
    /// <param name="animationName"></param>
    public void QueueAnimation(string animationName)
    {
        try
        {
            _queuedAnimation.Add(animationStates[GetIndexFromAnimationName(animationName)].animation);
        }
        catch (Exception)
        {
            Debug.LogWarning("Tried to find animation \"" + animationName + "\" but couldn't.");
        }

    }

    /// <summary>
    /// Clear the animation queue. This does not stop the current animation
    /// </summary>
    public void ClearQueue()
    {
        _queuedAnimation.Clear();
    }

    public List<SpriteAnimation> GetQueue()
    {
        return _queuedAnimation;
    }

    public string[] GetNamedQueue()
    {
        string[] queuedAnimNames = new string[_queuedAnimation.Count];
        int i = 0;
        foreach (SpriteAnimation anim in _queuedAnimation)
        {
            queuedAnimNames[i] = anim.name;
            i++;
        }

        return queuedAnimNames;
    }

    public SpriteAnimation GetCurrentAnimation()
    {
        return currentAnimation;
    }

    public string GetCurrentAnimationName()
    {
        return currentAnimation.name;
    }

    public SpriteAnimation[] GetAllAnimations()
    {
        SpriteAnimation[] animColl = new SpriteAnimation[animationStates.Length];

        int i = 0;

        foreach (AnimationState animHolder in animationStates)
        {
            animColl[i] = animHolder.animation;
            i++;
        }
        return animColl;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ResetAnimator()
    {
        ClearQueue();
        currentAnimation = animationStates[defaultAnimationState].animation;
        currentAnimationState = defaultAnimationState;

        SetFrame(0);
    }
}

[Serializable]
public class AnimationEventHandler
{
    public int frame;
    public UnityEvent animEvent;
}

[Serializable]
public class AnimationState
{
    public SpriteAnimation animation;
    public AnimationEventHandler[] events;
    [HideInInspector]
    public string name;
}