using System;
using UnityEngine;
using UnityEditor;
public abstract class CustomMono : MonoBehaviour
{
    public static Action<Vector2> OnScreenSizeChange;

    [InitializeOnLoadMethod]
    public virtual void AwakeEditor()
    {}
}