using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Delay
{
    MonoBehaviour caller;

    public Delay(MonoBehaviour caller, float delaySeconds, Action action)
    {
        this.caller = caller;
        caller.StartCoroutine(DoDelay(action, delaySeconds));
    }

    IEnumerator DoDelay(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        if (caller != null)
            action?.Invoke();
    }
}
