using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject playPanel;
    public GameObject settingsPanel;
    public Button settings;
    public Button backToPlay;// Start is called before the first frame update
    Animator animator;
    void Start()
    {
        //playPanel.gameObject.SetActive(true);
        //settingsPanel.gameObject.SetActive(false);
        settings = settings.GetComponent<Button>();
        settings.onClick.AddListener(OpenSettingsPanel);
        backToPlay = backToPlay.GetComponent<Button>();
        backToPlay.onClick.AddListener(BackToPlayPanel);
        animator = settingsPanel.GetComponent<Animator>();
        
    }

    void BackToPlayPanel()
    {
        animator.SetBool("isOpen", false);
        playPanel.gameObject.SetActive(true);
        //settingsPanel.gameObject.SetActive(false);
    }

    void OpenSettingsPanel()
    {
        //settingsPanel.gameObject.SetActive(true);
        animator.SetBool("isOpen", true);
        playPanel.gameObject.SetActive(false);
    }
}
