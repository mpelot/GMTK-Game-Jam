using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEventMasker : MonoBehaviour
{
    [SerializeField] LayerMask eventMask;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.eventMask = eventMask;
    }
}
