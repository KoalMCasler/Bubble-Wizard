using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEventTrigger : MonoBehaviour
{
    private ParticleSystem particle;
    private bool hasBeenTriggered;
    // Start is called before the first frame update
    void Start()
    {
        hasBeenTriggered = false;
        particle = GetComponent<ParticleSystem>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && !hasBeenTriggered)
        {
            particle.Play();
            hasBeenTriggered = true;
        }
    }
}
