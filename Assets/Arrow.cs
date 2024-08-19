using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject arrowHead;
    public GameObject arrowTail;
    

    public void SetSize(float size)
    {
        Vector3 baseArrowTailScale = arrowTail.transform.localScale;
        baseArrowTailScale.x = size;
        arrowTail.transform.localScale = baseArrowTailScale;
        arrowTail.transform.localPosition = new Vector3(size * 1.475f, 0, 0);
        arrowHead.transform.localPosition = new Vector3(size * 2.95206f, 0, 0);

    }
}
