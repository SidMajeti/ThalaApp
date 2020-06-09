using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstatiateGlobalVars : MonoBehaviour
{
    // instantiates android plugin classes

    AndroidJavaClass javaClass;
    AndroidJavaObject javaObject;
    AndroidJavaClass jc;
    void Start()
    {
        #if UNITY_ANDROID
            javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
            jc = new AndroidJavaClass("com.thalaapp.unityplugin.PlayAudio");
            jc.CallStatic("instantiateMp", javaObject, "tap.wav");
        #endif
    }

    public AndroidJavaClass GetUnityJavaClass()
	{
        return javaClass;
	}
    public AndroidJavaObject GetUnityJavaObject()
	{
        return javaObject;
	}
    public AndroidJavaClass GetPluginJavaClass()
	{
        return jc;
	}
}
