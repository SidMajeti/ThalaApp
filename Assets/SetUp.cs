using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : MonoBehaviour
{
    public Canvas errorPopup;// Start is called before the first frame update
    void Start()
    {
        errorPopup.enabled = false;
        errorPopup.GetComponent<CanvasGroup>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
