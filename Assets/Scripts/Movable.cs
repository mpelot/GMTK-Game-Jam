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
    public bool isBeingPulledToCore;
    private Core core;
    private bool clickInterrupt = false;
    public float coreForce;
    [SerializeField] private Animator boostAnim;
    public bool disableInteraction = false;
    [SerializeField] private Collider2D coll;
    [SerializeField] public Collider2D targetPositionMarkerCollider;
    public AudioSource audioSource;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        core = GameObject.FindFirstObjectByType<Core>();
    }

    private void Update() {
        if (disableInteraction)
            return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && coll.OverlapPoint(mousePos)) {
            clickInterrupt = false;
            StartCoroutine(ClickWait());
        }

        if (Input.GetMouseButtonUp(0)) {
            if (disableInteraction)
                return;
            if (!destinationSet)
                rb.drag = 4f;
            mouseDown = false;
            clickInterrupt = true;
        }

        if (mouseDown) {
            targetPos = new Vector3(mousePos.x, mousePos.y);
        }

        if (selected) {
            if (Input.GetMouseButtonDown(1) && !coll.OverlapPoint(mousePos)) {
                destinationSet = true;
                rb.drag = 2;
                targetPos = new Vector3(mousePos.x, mousePos.y);
            }
            if (Input.GetMouseButtonDown(1) && targetPositionMarkerCollider.OverlapPoint(mousePos)) {
                destinationSet = false;
                rb.drag = 4;
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
                audioSource.Pause();
            } else {
                forceVector += (Vector2) (targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0.5f, 1.5f) * (dragSpeed * dragSpeedMultiplier);
                transform.right = (targetPos - transform.position).normalized;
                boostAnim.SetBool("Boost", true);
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        } else if (destinationSet) {
            if ((targetPos - transform.position).magnitude < 0.01f) {
                transform.position = targetPos;
                destinationSet = false;
                rb.drag = 10;
                boostAnim.SetBool("Boost", false);
                audioSource.Pause();
            }
            else {
                forceVector += (Vector2) (targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0.5f, 1.5f) * (dragSpeed * dragSpeedMultiplier);
                transform.right = (targetPos - transform.position).normalized;
                boostAnim.SetBool("Boost", true);
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        } else {
            boostAnim.SetBool("Boost", false);
            audioSource.Pause();
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

    IEnumerator ClickWait() {
        yield return new WaitForSeconds(.1f);
        if (!clickInterrupt) {
            mouseDown = true;
            destinationSet = false;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.drag = 2f;
        }
    }
}
