using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CritWheel : MonoBehaviour
{
    //    float value = 0;
    //    [SerializeField] Slider slider;

    //    [SerializeField] float _speed;
    //    float _currentSpeed;

    //    [SerializeField] CritStage[] _stages;
    //    public CritStage CurrentStage { get => _stages[_stageIndex]; }
    //    int _stageIndex;

    //    [SerializeField] Image _image;

    //    [SerializeField] CritReward _initalHitReward;

    //    [SerializeField] AudioClip finishSound;

    //    [SerializeField] float _pauseTime = 0.5f;
    //    [SerializeField] float _randomness = 0.2f;

    //    public bool IsActive { get; private set; }

    //    List<IPlayerAttackable> _targets = new List<IPlayerAttackable>();

    //    public event Action OnFinish;

    //    private void Awake()
    //    {
    //        gameObject.SetActive(false);
    //        IsActive = false;
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        if (IsActive)
    //        {
    //            value += Time.deltaTime * _currentSpeed;
    //            if (value > 1f || value < 0f)
    //                Fail(true);

    //            slider.value = value;
    //        }
    //    }

    //    public CritReward Activate()
    //    {
    //        value = 0; 
    //        _currentSpeed = _speed + UnityEngine.Random.Range(-_randomness, _randomness);
    //        gameObject.SetActive(true);
    //        IsActive = true;
    //        _stageIndex = 0;
    //        _image.sprite = _stages[0].dial;
    //        return _initalHitReward;
    //    }

    //    public void Deactivate(bool instant)
    //    {
    //        OnFinish?.Invoke();
    //        OnFinish = null;

    //        IsActive = false;
    //        new Delay(Player.instance, 0.1f, FinishAttacks);
    //        if (instant)
    //        {
    //            gameObject.SetActive(false);
    //        }
    //        else {
    //            new Delay(Player.instance, _pauseTime, () => gameObject.SetActive(false));
    //        }
    //    }

    //    public CritReward Trigger()
    //    {
    //        bool insideHardSuccessRange = _currentSpeed < 0 ? value < CurrentStage.hardSuccessValue : value > CurrentStage.hardSuccessValue;

    //        if (insideHardSuccessRange)
    //        {
    //            CritReward reward = CurrentStage.hardCritReward;

    //            Success(2);
    //            return reward;
    //        }

    //        bool insideEasySuccessRange = _currentSpeed < 0 ? value < CurrentStage.easySuccessValue : value > CurrentStage.easySuccessValue;

    //        if (insideEasySuccessRange)
    //        {
    //            CritReward reward = CurrentStage.easyCritReward;

    //            Success(1);
    //            return reward;
    //        }

    //        Fail(false);
    //        return new CritReward();
    //    }

    //    private void Success(int stageIncrease)
    //    {
    //        SoundManager.PlaySound(CurrentStage.sound, transform, false);

    //        bool lastStage = _stageIndex >= 1;
    //        if (lastStage)
    //        {
    //            Deactivate(false);
    //            return;
    //        }

    //        // Next stage
    //        _stageIndex += stageIncrease;
    //        _image.sprite = CurrentStage.dial;
    //        _currentSpeed = -_currentSpeed;
    //    }

    //    void Fail(bool instant)
    //    {
    //        SoundManager.PlaySound(finishSound, transform, false);
    //        Deactivate(instant);
    //    }

    //    public void AddTarget(IPlayerAttackable target)
    //    {
    //        if (!_targets.Contains(target))
    //            _targets.Add(target);
    //    }

    //    void FinishAttacks()
    //    {
    //        foreach (IPlayerAttackable target in _targets)
    //        {
    //            //target.AttackFromPlayerEnd(Player.instance.CurrentWeapon);
    //        }
    //        _targets.Clear();
    //    }
    //}

    //[System.Serializable]
    //public class CritStage
    //{
    //    public float easySuccessValue;
    //    public float hardSuccessValue;

    //    public CritReward easyCritReward;
    //    public CritReward hardCritReward;

    //    public AudioClip sound;

    //    public Sprite dial;
}