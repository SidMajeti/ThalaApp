using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisableDropDownOptions : MonoBehaviour
{

    public Dropdown thalaDropDown;
    Dropdown kalaiDropdown;

    public void UpdateKalai()
    {
        //remove 2nd kalai option 
        if (thalaDropDown.value != 1 && kalaiDropdown.options.Count == 2)
        {
            //1st kalai automatically change
            kalaiDropdown.value = 0;
            kalaiDropdown.options.RemoveAt(1);
            kalaiDropdown.RefreshShownValue();
        }
        else if (kalaiDropdown.options.Count <= 1 && thalaDropDown.value == 1)
        {
            //Debug.Log("Entered");
            kalaiDropdown.options.Add(new Dropdown.OptionData() { text = "2" });
        }
    }

    void Awake()
    {
        kalaiDropdown = GetComponent<Dropdown>();
    }
    void Update()
    {
        //remove 2nd kalai option 
        if (thalaDropDown.value != 1 && kalaiDropdown.options.Count == 2)
        {
            //1st kalai automatically change
            kalaiDropdown.value = 0;
            kalaiDropdown.options.RemoveAt(1);
            kalaiDropdown.RefreshShownValue();
        }
        else if (kalaiDropdown.options.Count <= 1 && thalaDropDown.value == 1)
        {
            //Debug.Log("Entered");
            kalaiDropdown.options.Add(new Dropdown.OptionData() { text = "2" });
        }
    }
}
