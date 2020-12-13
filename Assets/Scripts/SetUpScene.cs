using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetUpScene : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_InputField inputField2;
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
