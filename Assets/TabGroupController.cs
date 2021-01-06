using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroupController : MonoBehaviour
{
    public Dropdown thalaDropdown;
    public Dropdown kalaiDropdown;
    public Dropdown sapta;
    public Dropdown jathi;
    public GameObject basicTab;
    public GameObject saptaTab;
    public GameObject handAnim;
    AndroidJavaObject jc;

    Animator animator;
    GameObject currentObj;
    GameObject otherObj;
    string previousName;
    void Start()
    {
        animator = handAnim.GetComponent<Animator>();
        previousName = "BasicTab";
        sapta.gameObject.SetActive(false);
        jathi.gameObject.SetActive(false);
#if UNITY_ANDROID
        jc = handAnim.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
#endif
    }

    public void onEnterAction(string name)
    {
        if (name.Equals("BasicTab"))
        {
            currentObj = basicTab;
            otherObj = saptaTab;
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("HandController");
            if (!previousName.Equals(name))
            {
                handAnim.GetComponent<AnimFuncs>().TaskOnValueChanged(0);
            }
            previousName = "BasicTab";
        }
        else
        {
            currentObj = saptaTab;
            otherObj = basicTab;
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AllThalas");
            if (!previousName.Equals(name))
            {
                handAnim.GetComponent<AnimFuncs>().TaskOnValueChanged(0);
            }
            previousName = "SaptaTab";
        }

        //change color of text and background for selected tab
        Color c;
        c.a = 255f/ 255;
        c.r = 48f / 255;
        c.g = 48f / 255;
        c.b = 48f / 255;
        currentObj.GetComponent<Image>().color = c;
        GameObject text = currentObj.transform.GetChild(0).gameObject;
        Color ct = new Color();
        ct.a = 255f / 255;
        ct.r = 255f / 255;
        ct.g = 255f / 255;
        ct.b = 255f / 255;
        Text t = text.GetComponent<Text>();
        t.color = ct;

        Color temp;
        temp.a = 128f / 255;
        temp.r = 255f / 255;
        temp.g = 255f / 255;
        temp.b = 255f / 255;
        currentObj.GetComponent<Outline>().effectColor = temp;
        Vector2 vector = new Vector2(6.72f, 6.35f);
        currentObj.GetComponent<Outline>().effectDistance = vector;
        //change color of text and background for other tab

        Color otherColor;
        otherColor.r = 255f / 255;
        otherColor.g = 255f / 255;
        otherColor.b = 255f / 255;
        otherColor.a = 128f / 255;
        otherObj.GetComponent<Image>().color = otherColor;
        GameObject othertext = otherObj.transform.GetChild(0).gameObject;
        Color c2;
        c2.a = 255f / 255;
        c2.r = 50f / 255;
        c2.g = 50f / 255;
        c2.b = 50f / 255;
        Text t2 = othertext.GetComponent<Text>();
        t2.color = c2;

        Color temp2;
        temp2.a = 128f / 255;
        temp2.r = 0f / 255;
        temp2.g = 0f / 255;
        temp2.b = 0f / 255;
        Vector2 vector2 = new Vector2(1f, -1f);
        otherObj.GetComponent<Outline>().effectColor = temp2;
        otherObj.GetComponent<Outline>().effectDistance= vector2;

        if (name.Equals("BasicTab"))
        {
            thalaDropdown.gameObject.SetActive(true);
            kalaiDropdown.gameObject.SetActive(true);
            sapta.gameObject.SetActive(false);
            jathi.gameObject.SetActive(false);
        }
        else
        {
            thalaDropdown.gameObject.SetActive(false);
            kalaiDropdown.gameObject.SetActive(false);
            sapta.gameObject.SetActive(true);
            jathi.gameObject.SetActive(true);
        }
    }


}
