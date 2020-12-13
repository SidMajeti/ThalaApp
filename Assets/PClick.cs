using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject handAnim;
    Animator animator;
    public Dropdown origThala;
    public Dropdown kalaiDropdown;
    public Dropdown fullThalas;
    public Dropdown jathis;
    public void OnPointerClick(PointerEventData eventData)
    {
        Dropdown thalaDropdown = gameObject.GetComponent<Dropdown>();
        Dropdown otherDropdown = null;
        animator = handAnim.GetComponent<Animator>();
        if (thalaDropdown.name.Equals("Thalas"))
        {
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AllThalas");
            otherDropdown = origThala;
        }
        else if(thalaDropdown.name.Equals("ThalaDropdown") || thalaDropdown.name.Equals("ThalaDropdown2"))
        {
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("HandController");
            otherDropdown = fullThalas;
        }
        List<string> optionList = new List<string>(thalaDropdown.options.Count - 1);
        for (int i = 0; i < optionList.Capacity; i++)
        {
            optionList.Add(string.Copy(thalaDropdown.options[i].text));
        }
        if (thalaDropdown.options[thalaDropdown.value].text.Equals("Sapta Thala") || thalaDropdown.options[thalaDropdown.value].text.Equals("General") || thalaDropdown.options[thalaDropdown.value].text.Equals("Kalai") || thalaDropdown.options[thalaDropdown.value].text.Equals("Jathi"))
        {
            if(thalaDropdown.options[thalaDropdown.value].text.Equals("Sapta Thala"))
            {
                Debug.Log("EnteredSapta");
                List<string> options = new List<string> { "General" };
                otherDropdown.AddOptions(options);
                otherDropdown.value = otherDropdown.options.Count - 1;
                otherDropdown.RefreshShownValue();
                otherDropdown.enabled = false;
                otherDropdown.enabled = true;
            }
            else if(thalaDropdown.options[thalaDropdown.value].text.Equals("General"))
            {
                Debug.Log("EnteredGeneral");
                List<string> options = new List<string> { "Sapta Thala" };
                otherDropdown.AddOptions(options);
                otherDropdown.value = otherDropdown.options.Count - 1;
                otherDropdown.RefreshShownValue();
                otherDropdown.enabled = false;
                otherDropdown.enabled = true;
            }
            thalaDropdown.ClearOptions();
            thalaDropdown.RefreshShownValue();
            thalaDropdown.AddOptions(optionList);
            thalaDropdown.value = 0;
            thalaDropdown.RefreshShownValue();
            //thalaDropdown.enabled = false;
            //thalaDropdown.enabled = true;
            //thalaDropdown.Show();

        }
        if (origThala.value != 1 && kalaiDropdown.options.Count == 2)
        {
            //Debug.Log("Entered");
            //1st kalai automatically change
            kalaiDropdown.value = 0;
            kalaiDropdown.options.RemoveAt(1);
            kalaiDropdown.RefreshShownValue();
        }
        else if (kalaiDropdown.options.Count <= 1 && origThala.value == 1)
        {
            //Debug.Log("Entered");
            kalaiDropdown.options.Add(new Dropdown.OptionData() { text = "2nd Kalai" });
        }
        //make other animator show default value
        thalaDropdown.enabled = false;
        thalaDropdown.enabled = true;
        thalaDropdown.Show();

    }

}
