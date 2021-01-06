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
    public GameObject saptaPanel;
    public Button settings;
    public Button backToPlay;// Start is called before the first frame update
    Animator animator;
    Animator fullSettingsAnim;
    Animator saptaSettingsAnim;
    public Button fullBackToPlay;
    public Button saptaPanelBack;
    //0 = basic panel
    //1 = sapta Panel
    public int current = 0;
    void Awake()
    {
        fullSettingsPanel.SetActive(false);
        settings = settings.GetComponent<Button>();
        settings.onClick.AddListener(OpenSettingsPanel);
        backToPlay = backToPlay.GetComponent<Button>();
        backToPlay.onClick.AddListener(BackToPlayPanel);
        animator = settingsPanel.GetComponent<Animator>();
        fullSettingsAnim = fullSettingsPanel.GetComponent<Animator>();
        saptaSettingsAnim = saptaPanel.GetComponent<Animator>();
        fullBackToPlay = fullBackToPlay.GetComponent<Button>();
        fullBackToPlay.onClick.AddListener(BackToPlayPanel);
        saptaPanelBack = saptaPanelBack.GetComponent<Button>();
        saptaPanelBack.onClick.AddListener(BackToPlayPanel);
    }

    void BackToPlayPanel()
    {
        //Debug.Log("BackPanel gets called");
        if (settingsPanel.gameObject.activeSelf == true)
        {
            animator.SetBool("isOpen", false);
        }
        else if(current == 0)
        {
            fullSettingsAnim.SetBool("isOpen", false);
        }
        else if (current == 1)
        {
            saptaSettingsAnim.SetBool("isOpenSapta", false);
        }
        playPanel.gameObject.SetActive(true);
        //settingsPanel.gameObject.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        //Debug.Log("Is Panel Active: " + settingsPanel.activeSelf);
        if (settingsPanel.gameObject.activeSelf == true)
        {
            animator.SetBool("isOpen", true);
        }
        else if(current == 0)
        {
            fullSettingsAnim.SetBool("isOpen", true);
        }
        else if(current == 1)
        {
            saptaSettingsAnim.SetBool("isOpenSapta", true);
        }
        //else if(sliderGeneral.value == 1)
        //{
        //    fullSettingsAnim.SetBool("isOpenSapta", true);
        //}
        //else if(sapta.value == 0)
        //{
        //    fullSettingsAnim.SetBool("isOpen", true);
        //}
        playPanel.gameObject.SetActive(false);
    }
}
