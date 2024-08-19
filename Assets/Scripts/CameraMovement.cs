using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float zoomStep, minSize, maxSize;

    [SerializeField] private float minX, maxX, minY, maxY;

    private Vector3 dragOrigin;

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        if (Input.mouseScrollDelta.y > .01f)
        {
            ZoomIn();
        }
        else if (Input.mouseScrollDelta.y < -.01f)
        {
            ZoomOut();
        }
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(2))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;

            ClampCameraPosition();
        }
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);

        ClampCameraPosition();
    }

    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);

        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float clampedX = Mathf.Clamp(cam.transform.position.x, minX + camWidth, maxX - camWidth);
        float clampedY = Mathf.Clamp(cam.transform.position.y, minY + camHeight, maxY - camHeight);

        cam.transform.position = new Vector3(clampedX, clampedY, cam.transform.position.z);
    }
}
