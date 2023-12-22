using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public SpriteRenderer borders;

    // Use this for initialization
    void Start()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = borders.bounds.size.x / borders.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = borders.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = borders.bounds.size.y / 2 * differenceInSize;
        }
    }
}
