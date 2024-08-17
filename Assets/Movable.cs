using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dragSpeed;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.drag = 2f;
    }

    private void OnMouseDrag() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 target = new Vector3(mousePos.x, mousePos.y);
        //transform.position = Vector3.MoveTowards(transform.position, target, 0.01f);
        rb.AddForce((target - transform.position).normalized * Mathf.Clamp((target - transform.position).magnitude, 0f, 1.5f) * dragSpeed, ForceMode2D.Force);
        //transform.position = new Vector3(Mathf.Clamp(mousePos.x - xOffset, -7f, 7f), Mathf.Clamp(mousePos.y - yOffset, -3.8f, 10f));
    }

    private void OnMouseUp() {
        rb.drag = 4f;
    }
}
