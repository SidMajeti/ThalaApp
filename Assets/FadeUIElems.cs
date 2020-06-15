using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUIElems : MonoBehaviour
{
    public CanvasGroup group;// Start is called before the first frame update

    public void Hide()
    {
        group.interactable = false;
        group.alpha = 0f;
        group.blocksRaycasts = false;
    }

    public void Show()
    {
        group.interactable = true;
        group.alpha = 1f;
        group.blocksRaycasts = true;
    }

    
}
