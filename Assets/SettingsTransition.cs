using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTransition : MonoBehaviour
{
    public Button settings;
    public Button back;
    Animator animator;
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        settings = settings.GetComponent<Button>();
        settings.onClick.AddListener(IsOpen);
        animator = GetComponent<Animator>();
        back = back.GetComponent<Button>();
        back.onClick.AddListener(IsNotOpen);
    }

    private void IsNotOpen()
    {
        animator.SetBool("isOpen", false);
    }

    private void IsOpen()
    {
        animator.SetBool("isOpen", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
