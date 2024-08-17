using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour, Selectable
{
    public GameObject ring;
    private Rigidbody2D rb;
    private bool selected = false;
    private GameMangager gm;
    private Movable moveable;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameMangager>();
        moveable = GetComponent<Movable>();
    }

    private void OnMouseDown() {
        if (!selected)
            gm.selectedObject = this;
    }

    public void Select() {
        selected = true;
        moveable.selected = true;
        ring.SetActive(true);
    }

    public void Deselect() {
        selected = false;
        moveable.selected = false;
        ring.SetActive(false);
    }
}
