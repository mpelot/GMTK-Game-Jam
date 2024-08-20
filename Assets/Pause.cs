using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool paused;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject pauseOption;

    public void pauseNow() {
        if (!paused) {
            SFXPlayer.instance.PlaySFX("MenuHover");
            pauseScreen.SetActive(true);
            pauseOption.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
        } else {
            SFXPlayer.instance.PlaySFX("MenuHover");
            pauseScreen.SetActive(false);
            pauseOption.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
        }
    }
}
