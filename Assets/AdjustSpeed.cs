using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdjustSpeed : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button incrSpeed;
    public Button decrSpeed;
    const int framesPerBeat = 20;
    const int framesPerSecond = 24;
    Animator m_Animator;
    double beats;

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
        inputField.text = Convert.ToString((int)(m_Animator.speed* 60 * framesPerSecond / framesPerBeat));
    }

    void ChangeSpeed(String val)
    {
        beats = float.Parse(val);
        float animSpeed = (float)(beats * framesPerBeat / (60 * framesPerSecond));
        m_Animator.speed = animSpeed;
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

    // Update is called once per frame
    void Update()
    {   
        
    }
}
