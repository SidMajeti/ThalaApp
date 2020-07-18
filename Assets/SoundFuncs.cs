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
#endif
    public Button volume;
    public Sprite mutedImage;
    public Sprite soundImage;
    bool mute = false;
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
            IOSMuteSound();
            mute = true;
            volume.GetComponent<Image>().sprite = mutedImage;
        }
        else
        {
            IOSUnMuteSound();
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
