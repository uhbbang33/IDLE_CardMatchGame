using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartBtn : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (gameObject.name == "StartBtnN" && PlayerPrefs.HasKey("normalscore"))
        {
            GetComponent<Button>().interactable = true;
        }
        else if (gameObject.name == "StartBtnH" && PlayerPrefs.HasKey("hardscore"))
        {
            GetComponent<Button>().interactable = true;
        }

    }

    public void GameStartE()
    {
        SceneManager.LoadScene("MainScene1");
    }

    public void GameStartN()
    {
        SceneManager.LoadScene("MainScene2");
    }

    public void GameStartH()
    {
        SceneManager.LoadScene("MainScene3");
    }
}
