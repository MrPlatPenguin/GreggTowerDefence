using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] Vector3 offset;

    private Vector3 velocity = Vector3.zero;
    Transform target;
    Vector3 shakePos= Vector3.zero;

    [SerializeField] Camera cam;
    float defaultCamSize;

    [SerializeField] AnimationCurve _shakeCurve;

    [SerializeField] float _lerpSpeed = 1f;

    private void Awake()
    {
        instance = this;
        PlayerEvents.OnPlayerSpawn += (Player player) => target = player.transform;
        PlayerEvents.OnPlayerSpawn += delegate { transform.position = target.position + offset; };

        PlayerEvents.OnEnterBase += OnEnterBase;
        PlayerEvents.OnExitBase += OnExitBase;
        LandUpgrader.OnUpgrade += () => UpdateCamSize(LandUpgrader.GetCurrentUpgrade().CamSize, _lerpSpeed);
        GameManager.OnGameOver += (string s) => Destroy(this);

        defaultCamSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime) + shakePos;
    }

    void OnEnterBase()
    {
        target = HomeBase.Instance.transform;
        UpdateCamSize(LandUpgrader.GetCurrentUpgrade().CamSize, _lerpSpeed);
    }

    void UpdateCamSize(float target, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(LerpCam(target, duration));
    }

    IEnumerator LerpCam(float target, float duration)
    {
        float t = 0;
        float runTime = 0;
        float startSize = cam.orthographicSize;
        while (t < 1)
        {
            runTime += Time.unscaledDeltaTime;
            t = Mathf.Clamp01(runTime/duration);
            cam.orthographicSize = Mathf.Lerp(startSize, target, t);
            yield return null;
        }
    }

    void OnExitBase()
    {
        target = Player.Transform;
        UpdateCamSize(defaultCamSize, _lerpSpeed);
    }

    public static void Shake(float duration, float magnatude)
    {
        instance.StartCoroutine(instance.DoShake(duration, magnatude));
    }

    IEnumerator DoShake(float duration, float magnatude)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float strength = _shakeCurve.Evaluate(timeElapsed / duration) * magnatude * Time.deltaTime;
            shakePos = Random.insideUnitSphere * strength;
            yield return null;
        }
        shakePos = Vector3.zero;
    }
}
