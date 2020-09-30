using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class SoundFuncs : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSMuteSound();
    [DllImport("__Internal")]
    private static extern void IOSUnMuteSound();
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
    [DllImport("__Internal")]
    private static extern bool IOSIsPlaying();
#endif
    public Button volume;
    public Sprite mutedImage;
    public Sprite soundImage;
    public bool mute = false;
    AndroidJavaObject jc;
    public Canvas canvas;
    void Start()
    {
        volume = volume.GetComponent<Button>();
        volume.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        #if UNITY_IOS
            if (mute == false)
            {
                if(IOSIsPlaying())
                    IOSStopSound();
                mute = true;
                volume.GetComponent<Image>().sprite = mutedImage;
            }
            else
            {
                mute = false;
                volume.GetComponent<Image>().sprite = soundImage;
            }
        #endif

#if UNITY_ANDROID
        jc = canvas.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
        if (mute == false)
        {
            jc.Call("stop");
            mute = true;
            volume.GetComponent<Image>().sprite = mutedImage;
        }
        else
        {
            //jc.CallStatic("playSound");
            mute = false;
            volume.GetComponent<Image>().sprite = soundImage;
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
