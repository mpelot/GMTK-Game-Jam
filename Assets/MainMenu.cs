using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MainMenuNow() {
        SceneManager.LoadSceneAsync(0);
        SceneManager.UnloadSceneAsync(1);
    }
}
