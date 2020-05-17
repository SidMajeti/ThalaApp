using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        //setting number of frames manually for target frame rate
        Application.targetFrameRate = 24;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
