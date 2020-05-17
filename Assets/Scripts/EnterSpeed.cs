using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnterSpeed : MonoBehaviour
{
    public Animator m_Animator;
    public InputField iField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void adjustSpeed()
	{
        float speed = float.Parse(iField.text);
        m_Animator.speed = speed;
	}

}
