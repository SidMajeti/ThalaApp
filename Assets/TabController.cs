using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabController : MonoBehaviour, IPointerClickHandler
{
    public GameObject settingsPanel;
    public Text text;

    void start()
    {
        //var outline = gameObject.AddComponent<Outline>();

        //outline.OutlineMode = Outline.Mode.OutlineAll;
        //outline.OutlineColor = Color.yellow;
        //outline.OutlineWidth = 5f;
        //outline.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(gameObject.name == "BasicTab")
        {
            text.text = "Kalai:";
        }
        else
        {
            text.text = "Jathi:";
        }
        settingsPanel.GetComponent<TabGroupController>().onEnterAction(gameObject.name);
    }

}
