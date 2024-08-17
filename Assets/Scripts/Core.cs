using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, Selectable
{
    private bool selected;
    private GameMangager gm;

    void Start() {
        gm = FindAnyObjectByType<GameMangager>();
    }

    private void OnMouseDown() {
        if (!selected)
            gm.selectedObject = this;
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
