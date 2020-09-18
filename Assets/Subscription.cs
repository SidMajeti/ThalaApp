using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subscription : MonoBehaviour
{
    public Dropdown thalaDropdown;
    public void onPurchase()
    {
        List<string> options = new List<string> { "Adi Thalam", "Khanda Chapu Thalam", "Misra Chapu Thalam", "Ata Thalam" };
        thalaDropdown.AddOptions(options);
    }
}
