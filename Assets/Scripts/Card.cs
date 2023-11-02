using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public AudioClip flip;  // 
    public AudioSource audioSource; //

    public Animator anim;

    public int spriteNum = 0;
    public void OpenCard()
    {
        CancelInvoke("CloseCard");
        CancelInvoke("CloseCardInvoke");
        if(GameManager.I.isRunning == true)
        {
            audioSource.PlayOneShot(flip);

            anim.SetBool("isOpen", true);

            transform.Find("Front").gameObject.SetActive(true);
            transform.Find("Back").gameObject.SetActive(false);

            if (GameManager.I.firstCard == null)
            {
                GameManager.I.firstCard = gameObject;
            }
            else
            {
                GameManager.I.secondCard = gameObject;
                GameManager.I.IsMatched();
            }

            //CloseCard 5초후에 닫힘
            Invoke("CloseCard", 2.5f);
        }
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
