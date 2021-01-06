using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetUpScene : MonoBehaviour
{
    public InputField inputField;
    public InputField inputField2;
    public Canvas canvas;
    //set up controls 
    void Start()
    {
        //ensures time scale is appropriate
        Time.timeScale = 1.0f;

    }

    // Update is called once per frame
    void Update()
    {
    }
    
}
