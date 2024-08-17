using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public GameObject ring;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnMouseDown() {
        ring.SetActive(true);
    }

    private void OnMouseUp() {
        ring.SetActive(false);
    }
}
