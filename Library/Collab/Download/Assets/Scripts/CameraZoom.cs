using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class CameraZoom : MonoBehaviour
{
    private PixelPerfectCamera ppcamera;
    //public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        ppcamera = GetComponent<PixelPerfectCamera>();
        //camera = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ppcamera.enabled = !ppcamera.enabled;
        }
    }

}
