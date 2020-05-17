using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class AnimFuncs : MonoBehaviour
{
    Animator m_Animator;
    public Button stop,start;
    public Dropdown dropdown;
    AnimatorControllerParameter[] parameters;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        stop = stop.GetComponent<Button>();
        start = start.GetComponent<Button>();
        stop.onClick.AddListener(TaskOnClick);
        start.onClick.AddListener(TaskOnClick);
        parameters = m_Animator.parameters;
    }

    void TaskOnClick()
    {
        if(EventSystem.current.currentSelectedGameObject.name == "StopButton")
        {
            m_Animator.SetBool("StopAnim", true);
            for(int i = 0; i < parameters.Length - 1; i++)
            {
                m_Animator.SetBool(parameters[i].name, false);
            }
               
        }
        else if(EventSystem.current.currentSelectedGameObject.name == "PlayButton")
        {
            m_Animator.SetBool("StopAnim", false);
            int val = dropdown.value;
            m_Animator.SetBool(parameters[val].name, true);
        }
    }

}
