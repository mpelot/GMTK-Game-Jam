using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPositionMarker : MonoBehaviour
{
    private Vector3 initSize;
    private void Start()
    {
        initSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float size = (Mathf.PingPong(Time.time, 0.5f) / 2.0f) + 1f;
        transform.localScale = initSize * size;
    }
}
