using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private GameMangager gm;

    private void OnMouseDown() {
        gm.selectedObject = null;
    }
}
