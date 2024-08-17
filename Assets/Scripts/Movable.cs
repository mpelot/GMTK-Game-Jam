using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dragSpeed;
    private bool destinationSet;
    private Camera cam;
    private Vector3 targetPos;
    public bool selected;
    private bool mouseDown = false;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Update() {
        if (mouseDown) {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector3(mousePos.x, mousePos.y);
        }

        if (selected) {
            if (Input.GetMouseButtonDown(1)) {
                destinationSet = true;
                rb.drag = 2;
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                targetPos = new Vector3(mousePos.x, mousePos.y);
            }
        }
    }

    private void FixedUpdate() {
        if (mouseDown) {
            if ((targetPos - transform.position).magnitude < 0.01f) {
                rb.drag = 10;
            } else {
                rb.AddForce((targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0f, 1.5f) * dragSpeed, ForceMode2D.Force);
            }
        }

        if (destinationSet) {
            if ((targetPos - transform.position).magnitude < 0.01f) {
                transform.position = targetPos;
                destinationSet = false;
                rb.drag = 10;
            } else {
                rb.AddForce((targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0f, 1.5f) * dragSpeed, ForceMode2D.Force);
            }
        }
    }

    private void OnMouseDown() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.drag = 2f;
        mouseDown = true;
        destinationSet = false;
    }

    private void OnMouseUp() {
        rb.drag = 4f;
        mouseDown = false;
    }
}
