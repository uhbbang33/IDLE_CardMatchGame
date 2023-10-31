using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public AudioClip flip;  // 음악 파일 자체
    public AudioSource audioSource; // 누가 음악을 플레이 할 것인지

    public Animator anim;
    
    public int spriteNum = 0;
    public void OpenCard()
    {
        audioSource.PlayOneShot(flip);

        anim.SetBool("isOpen", true);

        transform.Find("Front").gameObject.SetActive(true);
        transform.Find("Back").gameObject.SetActive(false);
        string swtime = GameManager.I.timeTxt.text;
 
        //closeCard 5초후 실행
        //Invoke("CloseCard", 5.0f);

        if (GameManager.I.firstCard == null)
        {
            GameManager.I.firstCard = gameObject;
        }
        else
        {
            GameManager.I.secondCard = gameObject;
            GameManager.I.IsMatched();
        }
    }

    public void DestrotyCard()
    {
        Invoke("DestroyCardInvoke", 0.5f);
    }

    void DestroyCardInvoke()
    {
        Destroy(gameObject);
    }

    public void CloseCard()
    {
        Invoke("CloseCardInvoke", 0.5f);
    }

    void CloseCardInvoke()
    {
        anim.SetBool("isOpen", false);
        transform.Find("Back").gameObject.SetActive(true);
        transform.Find("Front").gameObject.SetActive(false);

        if (gameObject == GameManager.I.firstCard)
            GameManager.I.firstCard = null;
    }

}
