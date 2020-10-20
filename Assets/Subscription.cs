using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subscription : MonoBehaviour
{
    public Dropdown thalaDropdown;
    public GameObject panel;

    //only gets called on every renewed subscription
    //directly interacts with apple store
    //so if not renewed yet and user kills and opens app, this functionality to still occur
    //acounted for this on the oninitialized function
    public void onPurchase()
    {
        List<string> options = new List<string> { "Adi Thalam", "Khanda Chapu Thalam", "Misra Chapu Thalam", "Ata Thalam" };
        thalaDropdown.AddOptions(options);
        Button button = GetComponent<Button>();
        GameObject b = button.gameObject;
        b.SetActive(false);
        panel.gameObject.SetActive(false);
        //Debug.Log("OnPurchase is called");
    }
}

