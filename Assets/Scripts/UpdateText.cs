using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text sliderValue;
    public Slider slider;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sliderValue.text = slider.value.ToString("0.0");
    }
}
