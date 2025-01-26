using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleParticle : MonoBehaviour
{
    public ParticleSystem particleSystem;
    private int particleCount;
    public SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(particleSystem.particleCount < particleCount)
        {
            soundManager.PlaySFX(5);
        }
        particleCount = particleSystem.particleCount;
    }
}
