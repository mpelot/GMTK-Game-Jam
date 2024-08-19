using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movable : MonoBehaviour
{
    private Rigidbody2D rb;
    public float dragSpeed;
    public float dragSpeedMultiplier = 1f;
    public bool destinationSet;
    private Camera cam;
    public Vector3 targetPos;
    public bool selected;
    private bool mouseDown = false;
    private bool mouseOver = false;
    public bool isBeingPulledToCore;
    private Core core;
    public float coreForce;
    [SerializeField] private Animator boostAnim;
    public bool disableInteraction = false;
    public Collider2D targetPositionMarkerCollider;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        core = GameObject.FindFirstObjectByType<Core>();
    }

    private void Update() {
        if (disableInteraction)
            return;
        if (mouseDown) {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector3(mousePos.x, mousePos.y);
        }

        if (selected) {
            if (Input.GetMouseButtonDown(1) && !mouseOver) {
                destinationSet = true;
                rb.drag = 2;
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                targetPos = new Vector3(mousePos.x, mousePos.y);
            }
        }

        if (destinationSet && selected) {
            targetPositionMarkerCollider.transform.position = targetPos;
            targetPositionMarkerCollider.transform.rotation = Quaternion.identity;
            targetPositionMarkerCollider.gameObject.SetActive(true);
        } else {
            targetPositionMarkerCollider.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {
        Vector2 forceVector = Vector2.zero;
        if (mouseDown) {
            if (disableInteraction)
                return;
            if ((targetPos - transform.position).magnitude < 0.01f) {
                rb.drag = rb.velocity.magnitude * 3f;
                boostAnim.SetBool("Boost", false);
            } else {
                forceVector += (Vector2) (targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0.5f, 1.5f) * (dragSpeed * dragSpeedMultiplier);
                transform.right = (targetPos - transform.position).normalized;
                boostAnim.SetBool("Boost", true);
            }
        } else if (destinationSet) {
            if ((targetPos - transform.position).magnitude < 0.01f) {
                transform.position = targetPos;
                destinationSet = false;
                rb.drag = 10;
                boostAnim.SetBool("Boost", false);
            } else {
                forceVector += (Vector2) (targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0.5f, 1.5f) * (dragSpeed * dragSpeedMultiplier);
                transform.right = (targetPos - transform.position).normalized;
                boostAnim.SetBool("Boost", true);
            }
        } else {
            boostAnim.SetBool("Boost", false);
        }

        if (isBeingPulledToCore)
        {
            Vector2 directionToCore = (core.transform.position - transform.position).normalized;
            Vector2 projection = (Vector2.Dot(forceVector, directionToCore) / Vector2.Dot(directionToCore, directionToCore)) * directionToCore;
            Vector2 perpendicular = forceVector - projection;

            forceVector = perpendicular + (directionToCore * coreForce);
        }

        rb.AddForce(forceVector, ForceMode2D.Force);
    }

    private void OnMouseDown() {
        if (disableInteraction)
            return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.drag = 2f;
        mouseDown = true;
        destinationSet = false;
    }

    private void OnMouseUp() {
        if (disableInteraction)
            return;
        rb.drag = 4f;
        mouseDown = false;
    }

    private void OnMouseEnter() {
        mouseOver = true;
    }

    private void OnMouseExit() {
        mouseOver = false;
    }
}
