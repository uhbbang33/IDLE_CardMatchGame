using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public GameObject btn;
    public GameObject settingUI;
    public GameObject deleteWarning;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setting()
    {
        settingUI.SetActive(true);
    }

    public void DataDelete()
    {
        PlayerPrefs.DeleteAll();
        deleteWarning.SetActive(true);
        CancelInvoke();
        Invoke("CloseWarning", 1.5f);
    }


    public void BackBtn()
    {
        settingUI.SetActive(false);
        SceneManager.LoadScene("StartScene");
    }

    public void CloseWarning()
    {
        deleteWarning.SetActive(false);
    }

}
