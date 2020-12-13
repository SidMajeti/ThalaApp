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
    [DllImport("__Internal")]
    private static extern bool IOSIsPlaying();
#endif
    Animator saptaAnimator;
    public Button start;
    public Dropdown thalaDropdown;
    public Dropdown saptaDropdown; 
    public Dropdown jathiDropdown;
    public Dropdown kalaiDropdown;
    public Dropdown thalaDropdown2;
    public Dropdown kalaiDropdown2;
    AnimatorControllerParameter[] parameters;
    public Canvas canvas;
    public bool isStopButton;
    public Sprite playImage;
    public Sprite stopImage;
    AndroidJavaObject jc;

    // Start is called before the first frame update
    void Awake()
    {
        start = start.GetComponent<Button>();
        start.onClick.AddListener(TaskOnClick);
        saptaAnimator = gameObject.GetComponent<Animator>();
        parameters = saptaAnimator.parameters;
        isStopButton = false;

        List<string> options = new List<string> { "General" };
        thalaDropdown.AddOptions(options);
        thalaDropdown.value = thalaDropdown.options.Count - 1;
        thalaDropdown.RefreshShownValue();

        List<string> options1 = new List<string> { "Sapta Thala" };
        saptaDropdown.AddOptions(options1);
        saptaDropdown.value = saptaDropdown.options.Count - 1;
        saptaDropdown.RefreshShownValue();


        List<string> options2 = new List<string> { "Kalai" };
        kalaiDropdown.AddOptions(options2);
        kalaiDropdown.value = kalaiDropdown.options.Count - 1;
        kalaiDropdown.RefreshShownValue();


        List<string> options3 = new List<string> { "Jathi" };
        jathiDropdown.AddOptions(options3);
        jathiDropdown.value = jathiDropdown.options.Count - 1;
        jathiDropdown.RefreshShownValue();

        saptaDropdown.onValueChanged.AddListener(TaskOnValueChanged);
        thalaDropdown.onValueChanged.AddListener(TaskOnValueChanged);
        jathiDropdown.onValueChanged.AddListener(TaskOnValueChanged);
        kalaiDropdown.onValueChanged.AddListener(TaskOnValueChanged);
        thalaDropdown2.onValueChanged.AddListener(TaskOnValueChanged);
        kalaiDropdown2.onValueChanged.AddListener(TaskOnValueChanged);
#if UNITY_ANDROID
        jc = m_Animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
#endif
    }

    void TaskOnValueChanged(int value)
    {
        //fix this logic!!
        saptaAnimator = gameObject.GetComponent<Animator>();
        int saptaParamlen;
        parameters = saptaAnimator.parameters;
        saptaAnimator.SetBool("StopAnim", true);
        if (saptaAnimator.runtimeAnimatorController.name.Equals("HandController"))
        {
            saptaParamlen = parameters.Length - 2;
        }
        else
        {
            saptaParamlen = parameters.Length - 1;
        }


        for (int i = 0; i < saptaParamlen; i++)
        {
            saptaAnimator.SetBool(parameters[i].name, false);
        }

        if (saptaAnimator.runtimeAnimatorController.name.Equals("HandController"))
        {
            saptaAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AllThalas");
            saptaParamlen = parameters.Length - 1;
        }
        else
        {
            saptaAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("HandController");
            saptaParamlen = parameters.Length - 2;
        }
        saptaAnimator.SetBool("StopAnim", true);

        for (int i = 0; i < saptaParamlen; i++)
        {
            saptaAnimator.SetBool(parameters[i].name, false);
        }

        if (saptaAnimator.runtimeAnimatorController.name.Equals("HandController"))
        {
            saptaAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AllThalas");
        }
        else
        {
            saptaAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("HandController");
        }

        bool isSubscribed = canvas.GetComponent<Purchaser>().isSubscribed;

        if (isSubscribed)
        {
            if (thalaDropdown2.value != 1 && kalaiDropdown2.options.Count == 2)
            {
                Debug.Log("Entered");
                //1st kalai automatically change
                kalaiDropdown2.value = 0;
                //kalaiDropdown2.options.RemoveAt(1);
                kalaiDropdown2.RefreshShownValue();
            }
            else if (kalaiDropdown2.options.Count <= 1 && thalaDropdown2.value == 1)
            {
                //Debug.Log("Entered");
                kalaiDropdown2.options.Add(new Dropdown.OptionData() { text = "2nd Kalai" });
            }
        }
        else
        {
            if (thalaDropdown.value != 1 && kalaiDropdown.options.Count == 2)
            {
                Debug.Log("Entered");
                Debug.Log("CountofKalai: " + kalaiDropdown.options.Count);
                //1st kalai automatically change
                kalaiDropdown.value = 0;
                //kalaiDropdown.options.RemoveAt(1);
                kalaiDropdown.RefreshShownValue();
            }
            else if (kalaiDropdown.options.Count <= 1 && thalaDropdown.value == 1)
            {
                //Debug.Log("Entered");
                kalaiDropdown.options.Add(new Dropdown.OptionData() { text = "2nd Kalai" });
            }
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
        start.GetComponent<RectTransform>().sizeDelta = new Vector2(159, 186);
        start.GetComponent<Image>().sprite = playImage;
    }

    void TaskOnClick()
    {
        saptaAnimator = gameObject.GetComponent<Animator>();
        parameters = saptaAnimator.parameters;
        if (isStopButton)
        {
            saptaAnimator.SetBool("StopAnim", true);
            for(int i = 0; i < parameters.Length - 2; i++)
            {
                saptaAnimator.SetBool(parameters[i].name, false);
            }
            isStopButton = false;
            start.GetComponent<Image>().color = Color.white;
            start.GetComponent<RectTransform>().sizeDelta = new Vector2(159, 186);
            start.GetComponent<Image>().sprite = playImage;
#if UNITY_IOS
           IOSStopSound();
#endif
#if UNITY_ANDROID
            while(jc.Call<bool>("isPlaying"))
                jc.Call("stop");
#endif
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //float currentTime = Time.time * 1000;
            //Debug.Log("Time when play is pressed : " +currentTime);
            saptaAnimator.SetBool("StopAnim", false);

            bool isSubscribed = canvas.GetComponent<Purchaser>().isSubscribed;
            
            if (saptaAnimator.runtimeAnimatorController.name.Equals("HandController"))
            {
                int value;
                int valofKalai;
                if (isSubscribed)
                {
                    value = thalaDropdown2.value;
                    valofKalai = kalaiDropdown2.value + 1;
                }
                else
                {
                    value = thalaDropdown.value;
                    valofKalai = kalaiDropdown.value + 1;
                }
                Debug.Log("Value" + value);
                saptaAnimator.SetBool(parameters[value].name, true);
                saptaAnimator.SetInteger("KalaiNum", valofKalai);
            }
            else
            {
                int val = saptaDropdown.value;
                int valofJathi = jathiDropdown.value;
                saptaAnimator.SetBool(parameters[val * jathiDropdown.options.Count + valofJathi].name, true);
            }

            isStopButton = true;
            start.GetComponent<RectTransform>().sizeDelta = new Vector2(159, 159);
            start.GetComponent<Image>().sprite = stopImage;
            //start.GetComponent<Image>().color = Color.white;
        }
    }

}
