using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeSpeed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool isPressed;
    public GameObject handAnim;
    public TMP_InputField inputField;
    Animator m_Animator;
    private float beats;



    // Start is called before the first frame update
    void Awake()
    {
        inputField = inputField.GetComponent<TMP_InputField>();
        m_Animator = handAnim.GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    void AlterSpeed(string val)
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
        if (isPressed)
        {
            beats = float.Parse(inputField.text);
            if (EventSystem.current.currentSelectedGameObject.name == "IncreaseSpeed")
            {
                beats += 1;
                AlterSpeed(Convert.ToString(beats));
                inputField.text = Convert.ToString(beats);
            }
            else if (EventSystem.current.currentSelectedGameObject.name == "DecreaseSpeed")
            {
                beats -= 1;
                AlterSpeed(Convert.ToString(beats));
                inputField.text = Convert.ToString(beats);
            }
        }
    }
}
