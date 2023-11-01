using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartBtn : MonoBehaviour
{
    private Button button;
    

    void Awake()
    {
        
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("bestscore"))
        {
            GetComponent<Button>().interactable = true;
        }
    }

    public void GameStartE()
    {
        GameManager.level = 0;
        SceneManager.LoadScene("MainScene1");
    }

    public void GameStartN()
    {
        GameManager.level = 1;
        SceneManager.LoadScene("MainScene2");
    }

    public void GameStartH()
    {
        GameManager.level = 2;
        SceneManager.LoadScene("MainScene3");
    }

    public void OnNormal()
    {
        GetComponent<Button>().interactable = true;
    }
}
