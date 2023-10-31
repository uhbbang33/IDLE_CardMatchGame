using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempScript : MonoBehaviour
{
    [SerializeField] Animator warningBGAnim;
    [SerializeField] AudioSource warningBGM;

    void Start()
    {
        warningBGAnim.SetBool("isWarning", true);
        warningBGM.Play();
        warningBGM.pitch = 2.0f;
    }
}
