using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePanel : MonoBehaviour
{
    // Start is called before the first frame update
    //public Slider basicSlider;
    //public Slider saptaSlider;
    public GameObject basicPanel;
    public GameObject saptaPanel;
    Animator fullSettingsAnim;
    Animator saptaSettingsAnim;
    void Awake()
    {
        //basicSlider = basicSlider.GetComponent<Slider>();
        ////saptaSlider = saptaSlider.GetComponent<Slider>();
        //basicSlider.onValueChanged.AddListener(updatePanel);
        //saptaSlider.onValueChanged.AddListener(updatePanel);
        fullSettingsAnim = basicPanel.GetComponent<Animator>();
        saptaSettingsAnim = saptaPanel.GetComponent<Animator>();
    }


    void updatePanel(float val)
    {
        //Debug.Log(val);
        if(val == 1)
        {
            //basicSlider.value = 0;
            gameObject.GetComponent<PanelController>().current = 1;
            fullSettingsAnim.SetBool("isOpen", false);
            gameObject.GetComponent<PanelController>().OpenSettingsPanel();
        }
        else
        {
            //saptaSlider.value = 1;
            gameObject.GetComponent<PanelController>().current = 0;
            saptaSettingsAnim.SetBool("isOpenSapta", false);
            gameObject.GetComponent<PanelController>().OpenSettingsPanel();
        }
    }
}
