using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject playPanel;
    public GameObject settingsPanel;
    public Button settings;
    public Button backToPlay;// Start is called before the first frame update
    void Start()
    {
        playPanel.gameObject.SetActive(true);
        settingsPanel.gameObject.SetActive(false);
        settings = settings.GetComponent<Button>();
        settings.onClick.AddListener(OpenSettingsPanel);
        backToPlay = backToPlay.GetComponent<Button>();
        backToPlay.onClick.AddListener(BackToPlayPanel);
    }

    void BackToPlayPanel()
    {
        playPanel.gameObject.SetActive(true);
        settingsPanel.gameObject.SetActive(false);
    }

    void OpenSettingsPanel()
    {
        playPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(true);
    }
}
