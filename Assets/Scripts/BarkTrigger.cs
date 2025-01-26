using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkTrigger : MonoBehaviour
{
    public Animator animator;
    private SoundManager soundManager;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            animator.SetTrigger("Bark");
            soundManager.PlaySFX(6);
        }
    }
}
