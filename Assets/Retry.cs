using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public void RetryNow() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
