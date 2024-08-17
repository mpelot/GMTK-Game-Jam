using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour, Selectable
{
    public GameObject ring;
    private Rigidbody2D rb;
    private bool selected = false;
    private GameMangager gm;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameMangager>();
    }

    private void OnMouseDown() {
        if (selected)
            Deselect();
        else
            gm.selectedObject = this;
    }

    public void Select() {
        selected = true;
        ring.SetActive(true);
    }

    public void Deselect() {
        selected = false;
        ring.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
        {
            Destroy(collision.gameObject);
            transform.localScale = new Vector3(transform.localScale.x + .1f, transform.localScale.y + .1f, 0f);
        }
    }
}
