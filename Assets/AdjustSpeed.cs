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
public class AdjustSpeed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
#if UNITY_IOS
    //[DllImport("__Internal")]
    //private static extern void IOSChangeSpeed(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
#endif
    public InputField inputField;
    public Button incrSpeed;
    public Button decrSpeed;
    public Button exitPopUp;
    const int framesPerBeat = 24;
    int framesPerSecond = 24;
    Animator m_Animator;
    double beats;
    public Canvas popUpError;
    public Canvas mainCanvas;
    AnimatorControllerParameter[] parameters;
    AudioSource audioSource;
    double offset;

    float time;

    public GameObject handAnim;
    bool isPressed;

    // Start is called before the first frame update
    void Awake()
    {
        //m_Animator.speed = animspeed;
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 24;
        Application.targetFrameRate = 60;
        incrSpeed = incrSpeed.GetComponent<Button>();
        decrSpeed = decrSpeed.GetComponent<Button>();
        inputField = inputField.GetComponent<InputField>();
        //inputField.contentType = InputField.ContentType.IntegerNumber;
        //inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        m_Animator = handAnim.GetComponent<Animator>();
        //incrSpeed.onClick.AddListener(TaskOnClick);
        //decrSpeed.onClick.AddListener(TaskOnClick);
        inputField.onEndEdit.AddListener(ChangeSpeed);
        inputField.text = Convert.ToString((int)(1/0.8 * m_Animator.speed * 60.0f));
        //exitPopUp.onClick.AddListener(ExitPopUp);
        //parameters = m_Animator.parameters;
        //audioSource = m_Animator.GetComponent<AudioSource>();
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

    public void TaskOnClick()
    {
        //Debug.Log("Getting pressed!");
        beats = float.Parse(inputField.text);
        if (EventSystem.current.currentSelectedGameObject.name == "IncreaseSpeed" || EventSystem.current.currentSelectedGameObject.name == "IncreaseSpeed2")
        {
            if (beats < 150.0f)
            {
                beats += 1;
                ChangeSpeed(Convert.ToString(beats));
                inputField.text = Convert.ToString(beats);
                //Debug.Log("Increasing speed");
            }
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "DecreaseSpeed" || EventSystem.current.currentSelectedGameObject.name == "DecreaseSpeed2")
        {
            if (beats > 50.0f)
            {
                beats -= 1;
                ChangeSpeed(Convert.ToString(beats));
                inputField.text = Convert.ToString(beats);
            }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        time = Time.time;
        StartCoroutine("dynamicSpeedUp");
    }

    IEnumerator dynamicSpeedUp()
    {
        while (isPressed)
        {
            TaskOnClick();
            yield return new WaitForSeconds((float)(1/(Time.time - time + 1) * 0.5));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        StopCoroutine("dynamicSpeedUp");
    }
}
