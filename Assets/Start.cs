using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color hoverColor;

    private void OnMouseEnter() {
        sr.color = hoverColor;
        SFXPlayer.instance.PlaySFX("MenuHover");
    }

    private void OnMouseExit() {
        sr.color = Color.white;
    }

    private void OnMouseDown() {
        SceneManager.LoadScene(1);
    }
}
