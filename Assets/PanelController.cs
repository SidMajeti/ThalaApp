using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject playPanel;
    public GameObject settingsPanel;
    public GameObject fullSettingsPanel;
    public Button settings;
    public Button backToPlay;// Start is called before the first frame update
    Animator animator;
    Animator fullSettingsAnim;
    public Button fullBackToPlay;
    void Awake()
    {
        fullSettingsPanel.SetActive(false);
        settings = settings.GetComponent<Button>();
        settings.onClick.AddListener(OpenSettingsPanel);
        backToPlay = backToPlay.GetComponent<Button>();
        backToPlay.onClick.AddListener(BackToPlayPanel);
        animator = settingsPanel.GetComponent<Animator>();
        fullSettingsAnim = fullSettingsPanel.GetComponent<Animator>();
        fullBackToPlay = fullBackToPlay.GetComponent<Button>();
        fullBackToPlay.onClick.AddListener(BackToPlayPanel);
    }

    void BackToPlayPanel()
    {
        Debug.Log("BackPanel gets called");
        if (settingsPanel.gameObject.activeSelf == true)
        {
            animator.SetBool("isOpen", false);
        }
        else
        {
            fullSettingsAnim.SetBool("isOpen", false);
        }
        playPanel.gameObject.SetActive(true);
        //settingsPanel.gameObject.SetActive(false);
    }

    void OpenSettingsPanel()
    {
        Debug.Log("Is Panel Active: " + settingsPanel.activeSelf);
        if (settingsPanel.gameObject.activeSelf == true)
        {
            animator.SetBool("isOpen", true);
        }
        else
        {
            fullSettingsAnim.SetBool("isOpen", true);
        }
        playPanel.gameObject.SetActive(false);
    }
}
