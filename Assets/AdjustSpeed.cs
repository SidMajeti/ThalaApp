using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//changes animation speed
//updates inputField with speed and stops audio when necessary
public class AdjustSpeed : MonoBehaviour
{
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
    }

    void ExitPopUp()
    {
        popUpError.enabled = false;
        mainCanvas.GetComponent<CanvasGroup>().interactable = true;
        inputField.ActivateInputField();
    }

    void ChangeSpeed(String val)
    {
        bool isNumeric = double.TryParse(val, out beats);
        if (!isNumeric)
        {
            popUpError.enabled = true;
            mainCanvas.GetComponent<CanvasGroup>().interactable = false;
            popUpError.GetComponent<CanvasGroup>().interactable = true;
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
