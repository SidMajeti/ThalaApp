using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldScript : MonoBehaviour
{
#if UNITY_IOS
    //[DllImport("__Internal")]
    //private static extern void IOSChangeSpeed(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
#endif

    public InputField inputField;
    public GameObject handAnim;
    Animator m_Animator;


    // Start is called before the first frame update
    void Awake()
    {
        inputField = inputField.GetComponent<InputField>();
        m_Animator = handAnim.GetComponent<Animator>();
        inputField.onEndEdit.AddListener(ChangeSpeed);
        inputField.text = Convert.ToString((int)(1 / 0.8 * m_Animator.speed * 60.0f));

    }

    void ChangeSpeed(string val)
    {
        float beats = 0.0f;
        if (val != "")
        {
            beats = float.Parse(val);
        }

        if (beats > 150.0f || beats < 50.0f)
        {
            inputField.ActivateInputField();
        }
        else
        {
            float animSpeed = (float)(0.8 * beats / 60.0);
            m_Animator.speed = animSpeed;
            m_Animator.SetBool("StopAnim", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.isFocused)
        {
            m_Animator.SetBool("StopAnim", true);
        }
    }
}
