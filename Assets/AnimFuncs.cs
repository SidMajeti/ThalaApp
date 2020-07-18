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
    public Dropdown dropdown;
    AnimatorControllerParameter[] parameters;
    public Canvas canvas;
    bool isStopButton;
    public Sprite playImage;
    public Sprite stopImage;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        start = start.GetComponent<Button>();
        start.onClick.AddListener(TaskOnClick);
        parameters = m_Animator.parameters;
        isStopButton = false;
    }

    void TaskOnClick()
    {
        if(isStopButton)
        {
            m_Animator.SetBool("StopAnim", true);
            for(int i = 0; i < parameters.Length - 1; i++)
            {
                m_Animator.SetBool(parameters[i].name, false);
            }
            isStopButton = false;
            start.GetComponent<Image>().color = Color.white;
            start.GetComponent<Image>().sprite = playImage;
            #if UNITY_IOS
                IOSStopSound();
            #endif
        }
        else
        {
            m_Animator.SetBool("StopAnim", false);
            int val = dropdown.value;
            m_Animator.SetBool(parameters[val].name, true);
            isStopButton = true;
            start.GetComponent<Image>().sprite = stopImage;
            start.GetComponent<Image>().color = Color.red;
        }
    }

    void Update()
    {
        
    }

}
