using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour
{

    public float verticalFoV = 52.0f;
    private Camera camera;
    void Update()
    {
        camera = GetComponent<Camera>();
        camera.fieldOfView = calcVerticalFOV(verticalFoV, camera.aspect);
    }

    private float calcVerticalFOV(float hFOVInDeg, float aspectRatio)
    {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
        float vFOVInRads = 2 * Mathf.Atan((Mathf.Tan(hFOVInRads / 2) * aspectRatio));
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }

}
