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
        if (gameObject.name == "StartBtnN" && PlayerPrefs.HasKey("easyscore"))
        {
            GetComponent<Button>().interactable = true;
        }
        else if (gameObject.name == "StartBtnH" && PlayerPrefs.HasKey("normalscore"))
        {
            GetComponent<Button>().interactable = true;
        }

    }

    public void GameStartE()
    {
        SceneManager.LoadScene("EasyScene");
    }

    public void GameStartN()
    {
        SceneManager.LoadScene("NormalScene");
    }

    public void GameStartH()
    {
        SceneManager.LoadScene("HardScene");
    }
}
