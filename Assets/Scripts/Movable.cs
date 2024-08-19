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
    public bool _selected;
    private float selectedTimer;
    public bool selected
    {
        get
        {
            return _selected;
        }
        set
        {
            _selected = value;
            if (_selected)
            {
                selectedTimer = 0;
            }
        }
    }
    public bool isBeingPulledToCore;
    private Core core;
    public float coreForce;
    [SerializeField] private Animator boostAnim;
    public bool disableInteraction = false;
    public Collider2D targetPositionMarkerCollider;
    private Collider2D coll;
    private Vector2 mouseDownLocation;
    private bool readyToMoveTargetPos = false;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        core = GameObject.FindFirstObjectByType<Core>();
        coll = GetComponent<Collider2D>();
        mouseDownLocation = Vector2.zero;
    }

    private void Update() {
        if (disableInteraction)
        {
            targetPositionMarkerCollider.gameObject.SetActive(false);
            return;
        }

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (selected)
            {
                if (coll.OverlapPoint(mousePos) || targetPositionMarkerCollider.OverlapPoint(mousePos))
                {
                    readyToMoveTargetPos = true;
                    mouseDownLocation = (Vector2)mousePos;
                }
            }
        }

        float distanceToMouseDownLocation = (mouseDownLocation - (Vector2)mousePos).magnitude;
        if (Input.GetMouseButton(0) && readyToMoveTargetPos)
        {
            if (distanceToMouseDownLocation > 0.1f)
            {
                if ((mousePos - transform.position).magnitude > 1f)
                {
                    targetPos = new Vector3(mousePos.x, mousePos.y);
                    destinationSet = true;
                }
            } else
            {
                if (selected && selectedTimer > 0.2f)
                {
                    destinationSet = false;
                }
            }
            
        }

        if ((targetPos - transform.position).magnitude > 1f && destinationSet && selected)
        {
            targetPositionMarkerCollider.transform.position = targetPos;
            targetPositionMarkerCollider.transform.rotation = Quaternion.identity;
            targetPositionMarkerCollider.gameObject.SetActive(true);
        }
        else
        {
            targetPositionMarkerCollider.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            readyToMoveTargetPos = false;
        }

        selectedTimer += Time.deltaTime;
    }

    private void FixedUpdate() {
        Vector2 forceVector = Vector2.zero;
        if (destinationSet) {
            if ((targetPos - transform.position).magnitude < 0.01f) {
                transform.position = targetPos;
                destinationSet = false;
                rb.drag = 10;
                boostAnim.SetBool("Boost", false);
            } else {
                forceVector += (Vector2) (targetPos - transform.position).normalized * Mathf.Clamp((targetPos - transform.position).magnitude, 0f, 1.5f) * (dragSpeed * dragSpeedMultiplier);
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
}
