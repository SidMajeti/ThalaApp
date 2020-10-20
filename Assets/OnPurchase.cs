using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPurchase : MonoBehaviour
{
    public Button subsButton;
    // Start is called before the first frame update
    void Awake()
    {
        subsButton = subsButton.GetComponent<Button>();

    }

}
