using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotateSpeed = 200f;
    public Image circle;
    public Image lCircle;
    public Canvas mainCanvas;

    void Start()
    {
        var tempColor = circle.color;
        var tempColor1 = lCircle.color;
        tempColor.a = 0;
        tempColor1.a = 0;
        circle.color = tempColor;
        lCircle.color = tempColor1;
        rectComponent = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (mainCanvas.GetComponent<Purchaser>().loadCircle)
        {
            var tempColor = circle.color;
            var tempColor1 = lCircle.color;
            tempColor.a = 1;
            tempColor1.a = 1;
            circle.color = tempColor;
            lCircle.color = tempColor1;
            rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
        else
        {
            var tempColor = circle.color;
            var tempColor1 = lCircle.color;
            tempColor.a = 0;
            tempColor1.a = 0;
            circle.color = tempColor;
            lCircle.color = tempColor1;
        }
    }
}
