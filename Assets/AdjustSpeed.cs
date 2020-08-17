using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//changes animation speed
//updates inputField with speed and stops audio when necessary
public class AdjustSpeed : MonoBehaviour
{
#if UNITY_IOS
    //[DllImport("__Internal")]
    //private static extern void IOSChangeSpeed(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
#endif
    public TMP_InputField inputField;
    public Button incrSpeed;
    public Button decrSpeed;
    public Button exitPopUp;
    const int framesPerBeat = 20;
    const int framesPerSecond = 24;
    Animator m_Animator;
    double beats;
    public Canvas popUpError;
    public Canvas mainCanvas;
    AnimatorControllerParameter[] parameters;
    AudioSource audioSource;
    double offset;

    // Start is called before the first frame update
    void Start()
    {
        incrSpeed = incrSpeed.GetComponent<Button>();
        decrSpeed = decrSpeed.GetComponent<Button>();
        inputField = inputField.GetComponent<TMP_InputField>();
        m_Animator = gameObject.GetComponent<Animator>();
        incrSpeed.onClick.AddListener(TaskOnClick);
        decrSpeed.onClick.AddListener(TaskOnClick);
        inputField.onEndEdit.AddListener(ChangeSpeed);
        inputField.text = Convert.ToString((int)(m_Animator.speed * 60 * framesPerSecond / framesPerBeat)); ;
        exitPopUp.onClick.AddListener(ExitPopUp);
        parameters = m_Animator.parameters;
        audioSource = m_Animator.GetComponent<AudioSource>();
    }

    void ExitPopUp()
    {
        popUpError.enabled = false;
        mainCanvas.GetComponent<CanvasGroup>().interactable = true;
        inputField.ActivateInputField();
    }

    void ChangeSpeed(String val)
    {
        float beats = 0.0f;
        if (val != "")
        {
            beats = float.Parse(val);
        }

        if (beats > 200.0f || beats < 45.0f)
        {
            inputField.ActivateInputField();
        }
        else
        {
            float animSpeed = (float)(beats * framesPerBeat / (60 * framesPerSecond));
            m_Animator.speed = animSpeed;
            m_Animator.SetBool("StopAnim", false);
        }
        
    }

    void TaskOnClick()
    {
        beats = float.Parse(inputField.text);
        if (EventSystem.current.currentSelectedGameObject.name == "IncreaseSpeed")
        {
            beats += 1;
            ChangeSpeed(Convert.ToString(beats));
            inputField.text = Convert.ToString(beats);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "DecreaseSpeed") {
            beats -= 1;
            ChangeSpeed(Convert.ToString(beats));
            inputField.text = Convert.ToString(beats);
        }
    }

    // if people are typing in InputField, then stop audio
    void Update()
    {
        if (inputField.isFocused)
        {
            m_Animator.SetBool("StopAnim", true);
        }
    }
}
