using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector2 Rotate(this Vector2 vector, float angle)
    {
        angle *= Mathf.Deg2Rad;
        float x = (vector.x * Mathf.Cos(angle)) - (vector.y * Mathf.Sin(angle));
        float y = (vector.x * Mathf.Sin(angle)) + (vector.y * Mathf.Cos(angle));
        return new Vector2(x, y);
    }

    public static float DotToDeg(this float dot)
    {
        return (1 - dot) * 90;
    }

    public static void SetLifeTime(this GameObject gameObject, MonoBehaviour monoBehaviour, float duration)
    {
        new Delay(monoBehaviour, duration, () => GameObject.Destroy(gameObject));
        //DestroyAfterTime i = gameObject.AddComponent<DestroyAfterTime>();
        //i.Duration = duration;
    }

    public static int GetNthDidget(this int num, int n)
    {
        string str = num.ToString(); // convert the integer to a string
        char digit = str[n]; // get the character at the desired index
        int digitInt = Int32.Parse(digit.ToString()); // convert the character back to an integer

        return digitInt;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
