using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class AnimFuncs : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSPlaySound(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
#endif
    Animator m_Animator;
    public Button start;
    public Dropdown thalaDropdown;
    public Dropdown kalaiDropdown;
    AnimatorControllerParameter[] parameters;
    public Canvas canvas;
    public bool isStopButton;
    public Sprite playImage;
    public Sprite stopImage;
    AndroidJavaObject jc;

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        m_Animator = gameObject.GetComponent<Animator>();
        start = start.GetComponent<Button>();
        start.onClick.AddListener(TaskOnClick);
        parameters = m_Animator.parameters;
        isStopButton = false;
        thalaDropdown.onValueChanged.AddListener(TaskOnValueChanged);
        kalaiDropdown.onValueChanged.AddListener(TaskOnValueChanged);
#if UNITY_ANDROID
        jc = m_Animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
#endif
    }

    void TaskOnValueChanged(int value)
    {
        m_Animator.SetBool("StopAnim", true);   
        for (int i = 0; i < parameters.Length - 2; i++)
        {
            m_Animator.SetBool(parameters[i].name, false);
        }
#if UNITY_IOS
        IOSStopSound();
#endif
#if UNITY_ANDROID
        while(jc.Call<bool>("isPlaying"))
            jc.Call("stop");
#endif
        isStopButton = false;
        start.GetComponent<Image>().color = Color.white;
        start.GetComponent<Image>().sprite = playImage;
    }

    void TaskOnClick()
    {
        if(isStopButton)
        {
            m_Animator.SetBool("StopAnim", true);
            for(int i = 0; i < parameters.Length - 2; i++)
            {
                m_Animator.SetBool(parameters[i].name, false);
            }
            isStopButton = false;
            start.GetComponent<Image>().color = Color.white;
            start.GetComponent<Image>().sprite = playImage;
#if UNITY_IOS
                IOSStopSound();
#endif
#if UNITY_ANDROID
            while(jc.Call<bool>("isPlaying"))
                jc.Call("stop");
#endif
        }
        else
        {
            //float currentTime = Time.time * 1000;
            //Debug.Log("Time when play is pressed : " +currentTime);
            m_Animator.SetBool("StopAnim", false);
            int val = thalaDropdown.value;
            int valofKalai = kalaiDropdown.value + 1;
            m_Animator.SetBool(parameters[val].name, true);
            m_Animator.SetInteger("KalaiNum", valofKalai);
            isStopButton = true;
            start.GetComponent<Image>().sprite = stopImage;
            start.GetComponent<Image>().color = Color.red;
        }
    }

    void Update()
    {
        
    }

}
