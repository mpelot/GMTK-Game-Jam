using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Selectable {
    void Select();
    void Deselect();
    GameObject gameObj { get; }
}
