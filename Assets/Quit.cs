using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color hoverColor;
    public GameObject pressDelKey;

    private void Update()
    {
        if (PlayerPrefs.GetInt("SkipTutorial") == 1)
        {
            pressDelKey.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                PlayerPrefs.SetInt("SkipTutorial", 0);
                SFXPlayer.instance.PlaySFX("MenuHover");
            }
        }
        else
        {
            pressDelKey.SetActive(false);
        }
    }

    private void OnMouseEnter() {
        sr.color = hoverColor;
        SFXPlayer.instance.PlaySFX("MenuHover");
    }

    private void OnMouseExit() {
        sr.color = Color.white;
    }

    private void OnMouseDown() {
        Application.Quit();
    }
}
