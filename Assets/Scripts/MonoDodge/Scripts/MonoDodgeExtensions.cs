using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoDodgeExtensions
{
    public static T GetDodge<T>(this GameObject obj)  where T : MonoDodge
    {
        MonoDodgeComponent[] dodges = obj.GetComponents<MonoDodgeComponent>();
        if (dodges.Length < 1) {Debug.LogWarning("Requested a dodge from a gameobject that has none!"); return null;}
        for (int i = 0; i < dodges.Length; i++)
        {
            T instance = dodges[i].GetInstance<T>();
            if (instance != null) return instance;
        }

        return null;
    }
}
