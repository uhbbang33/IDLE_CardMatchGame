using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndText : MonoBehaviour
{
    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
