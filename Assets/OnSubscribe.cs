using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OnSubscribe : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
    [DllImport("__Internal")]
    private static extern bool IOSIsPlaying();

#endif
    public Button b;
    public GameObject handAnim;
    Animator animator;
    AnimatorControllerParameter[] parameters;
    public GameObject mainCanvas;
    AndroidJavaObject jc;
    // Start is called before the first frame update
    void Awake()
    {
        b = b.GetComponent<Button>();
        b.onClick.AddListener(ActionOnClick);
        animator = handAnim.GetComponent<Animator>();
        parameters = animator.parameters;
#if UNITY_ANDROID
        jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
#endif
    }

    void ActionOnClick()
    {
#if UNITY_ANDROID
        jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
#endif
        animator.SetBool("StopAnim", true);
        for (int i = 0; i < parameters.Length - 2; i++)
        {
            animator.SetBool(parameters[i].name, false);
        }
        animator.GetComponent<AnimFuncs>().isStopButton = false;
        animator.GetComponent<AnimFuncs>().start.GetComponent<Image>().color = Color.white;
        animator.GetComponent<AnimFuncs>().start.GetComponent<Image>().sprite = animator.GetComponent<AnimFuncs>().playImage;
#if UNITY_IOS
        IOSStopSound();
#endif
#if UNITY_ANDROID
            while(jc.Call<bool>("isPlaying"))
                jc.Call("stop");
#endif
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        mainCanvas.GetComponent<Purchaser>().BuySubscription();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
