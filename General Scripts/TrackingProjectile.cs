using Unity.VisualScripting;
using UnityEngine;

public class TrackingProjectile : MonoBehaviour
{
    Transform[] targets;
    float movementSpeed = 2f;   // Constant movement speed
    private int currentWaypointIndex = 0;
    private Transform currentTarget;
    private Transform prevTarget;
    private float journeyLength;
    private float startTime;
    float journeyFraction;
    [SerializeField] SoundClip returnSound;


    public void Init(Transform[] transforms, float speed)
    {
        this.targets = transforms;
        movementSpeed = speed;
        currentWaypointIndex = 1;
        prevTarget= targets[0];
        currentTarget = targets[currentWaypointIndex];
        journeyLength = Vector3.Distance(transform.position, currentTarget.position);
        startTime = Time.time;
    }

    private void Update()
    {
        if (targets[0].IsDestroyed())
        {
            Destroy(gameObject);
            return;
        }

        float distanceCovered = (Time.time - startTime) * movementSpeed;
        journeyFraction = distanceCovered / journeyLength;

        transform.position = Vector3.Lerp(prevTarget.position, currentTarget.position, journeyFraction);

        if (journeyFraction >= 1f)
        {
            prevTarget = currentTarget;
            currentWaypointIndex++;
            if (currentWaypointIndex >= targets.Length)
            {
                Destroy(gameObject);
                return;
            }

            currentTarget = targets[currentWaypointIndex];
            journeyLength = Vector3.Distance(transform.position, currentTarget.position);
            startTime = Time.time;
            SoundManager.PlaySound(returnSound, transform, false);
        }
    }
}