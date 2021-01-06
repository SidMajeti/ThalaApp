using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class InstatiateGlobalVars : MonoBehaviour
{
    // instantiates android plugin classes
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void InitSPlayer(string otherSoundPath, string firstSoundPath);
#endif
#if UNITY_ANDROID
    AndroidJavaClass javaClass;
    AndroidJavaObject javaObject;
    AndroidJavaClass jc;
    AndroidJavaObject jcInstance;
#endif
    void Awake()
    {
        Application.runInBackground = false;
#if UNITY_ANDROID
            javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
            jc = new AndroidJavaClass("com.majeti.unity.Metronome");
            jcInstance = jc.GetStatic<AndroidJavaObject>("myInstance");
            jcInstance.Call("initSoundFile", javaObject, "high_bell.wav", "Rest.wav");
#endif
#if UNITY_IOS
        string firstSound = Path.Combine(Application.streamingAssetsPath, "high_bell.wav");
        string otherSound = Path.Combine(Application.streamingAssetsPath, "Rest.wav");
        InitSPlayer(otherSound, firstSound);
        #endif
    }

#if UNITY_ANDROID
    public AndroidJavaClass GetUnityJavaClass()
	{
        return javaClass;
	}
    public AndroidJavaObject GetUnityJavaObject()
	{
        return javaObject;
	}
    public AndroidJavaObject GetPluginJavaClass()
	{
        return jcInstance;
	}
#endif
}
