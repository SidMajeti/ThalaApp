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
    private static extern void InitSPlayer(string sFilePath);
#endif
#if UNITY_ANDROID
    AndroidJavaClass javaClass;
    AndroidJavaObject javaObject;
    AndroidJavaClass jc;
    AndroidJavaObject jcInstance;
#endif
    void Start()
    {
        #if UNITY_ANDROID
            javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            javaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
            jc = new AndroidJavaClass("com.thalaapp.unityplugin.Metronome");
            jcInstance = jc.CallStatic<AndroidJavaObject>("getInstance");
            jcInstance.Call("initMetroTask", jcInstance);
            //Debug.Log("instantiated class properly");
            jcInstance.Call("setBeatSound", 2440.0);
            //Debug.Log("Beat sound: " + bs);
            //Debug.Log("called beat sound method");
            jcInstance.Call("setSound", 6440.0);
            jcInstance.Call("setBeat", 8);
        #endif
        #if UNITY_IOS
            string StreamingAssetsBundlesFolder = Path.Combine(Application.streamingAssetsPath, "snap1.wav");
            InitSPlayer(StreamingAssetsBundlesFolder);
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
