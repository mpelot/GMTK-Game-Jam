using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MainMenuNow() {
        SFXPlayer.instance.PlaySFX("MenuHover");
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
        SceneManager.UnloadSceneAsync(1);
    }
}
