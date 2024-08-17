using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, Selectable
{
    private bool selected;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Contains("Asteroid")) {
            Destroy(collision.gameObject);
            transform.localScale = new Vector3(transform.localScale.x + .1f, transform.localScale.y + .1f, 0f);
        }
    }

    private void OnMouseDown() {
        Select();
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
