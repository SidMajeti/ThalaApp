using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTime : MonoBehaviour
{
    float pastTime = 0;
    public void setTime(float time)
    {
        pastTime = time;
    }
    public float getTime()
    {
        return pastTime;
    }
}
