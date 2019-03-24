using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Log
{
    public static void Trace(string TAG,params string[] msg)
    {
        UnityEngine.Debug.Log(PackageMsg(TAG, msg));
    }

    public static void Warning(string TAG, params string[] msg)
    {
        UnityEngine.Debug.LogWarning(PackageMsg(TAG, msg));
    }

    public static void Info(string TAG, params string[] msg)
    {
        UnityEngine.Debug.Log(PackageMsg(TAG, msg));
    }

    public static void Error(string TAG, Exception e)
    {
        UnityEngine.Debug.LogError(PackageMsg(TAG, e.ToString()));
    }

    public static void Error(string TAG, params string[] msg)
    {
        UnityEngine.Debug.LogError(PackageMsg(TAG, msg));
    }

    public static void Debug(string TAG, params string[] msg)
    {
        UnityEngine.Debug.Log(PackageMsg(TAG, msg));
    }

    private static string PackageMsg(string TAG, params string[] msg)
    {
        string ms = "[" + TAG + "]:";
        for (int i = 0; i < msg.Length; i++)
        {
            ms += msg[i];
        }
        return ms;
    }
}
