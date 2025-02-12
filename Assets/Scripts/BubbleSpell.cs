using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleSpell : MonoBehaviour
{
    public SoundManager soundManager;
    public Rigidbody2D bubbleRb;
    public float velocityThreshold;
    public Vector3 emptyScale;
    public Vector3 capturedScale;
    public bool hasExpanded;
    public PlayerController player;
    public bool isFacingLeft;
    public GameObject capturedTarget;
    public GameObject popParticles;
    public float colliderDelayTime;
    public ParticleSystem particleSystem;
    private int particleCount;
    // Start is called before the first frame update
    void Start()
    {
        bubbleRb = this.GetComponent<Rigidbody2D>();
        hasExpanded = false;
        player = FindObjectOfType<PlayerController>();
        this.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("ColliderDelay");
        transform.localScale = Vector3.one;
        particleSystem = GetComponent<ParticleSystem>();
        soundManager = FindObjectOfType<SoundManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if(isFacingLeft)
        {
            if(bubbleRb.velocity.x > -velocityThreshold && !hasExpanded)
            {
                bubbleRb.velocity = Vector2.zero;
                ExpandEmpty();
            }
        }
        else
        {
            if(bubbleRb.velocity.x < velocityThreshold && !hasExpanded)
            {
                bubbleRb.velocity = Vector2.zero;
                ExpandEmpty();
            }
        }
        if(capturedTarget != null)
        {
            capturedTarget.transform.position = this.transform.position;
        }
        var amount = Mathf.Abs(particleCount - particleSystem.particleCount);
        if(particleSystem.particleCount < particleCount)
        {
            soundManager.PlaySFX(5);
        }
        particleCount = particleSystem.particleCount;
    }

    void TrapTarget()
    {
        soundManager.PlaySFX(3);
        transform.localScale = Vector3.zero;
        transform.DOScale(capturedScale, 1f).SetEase(Ease.OutElastic);
        hasExpanded = true;
    }

    void ExpandEmpty()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(emptyScale, 1f).SetEase(Ease.OutElastic);
        hasExpanded = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "Moveable")
        {
            TrapTarget();
            capturedTarget = other.gameObject;
            capturedTarget.GetComponent<Collider2D>().enabled = false;
            capturedTarget.GetComponent<Rigidbody2D>().simulated = false;
            if(other.gameObject.tag == "Enemy")
            {
                if(capturedTarget.GetComponent<EnemyAI>().isOnPatrol)
                {
                    capturedTarget.GetComponent<EnemyAI>().isOnPatrol = false;
                    capturedTarget.GetComponent<EnemyAI>().wasOnPatrol = true;
                    capturedTarget.GetComponent<EnemyAI>().enemyAnim.SetBool("isOnPatrol",capturedTarget.GetComponent<EnemyAI>().isOnPatrol);
                }
            }
        }
        else if(other.gameObject.tag == "Player")
        {
            if(other.transform.position.y > transform.position.y)
            {
                player.Bounce();
                Destroy(player.activeBubble);
            }
        }
    }

    void ReleaseTarget()
    {
        capturedTarget.GetComponent<Collider2D>().enabled = true;
        capturedTarget.GetComponent<Rigidbody2D>().simulated = true;
    }

    void OnDestroy()
    {
        soundManager.PlaySFX(2);
        GameObject burst = Instantiate(popParticles,transform.position,transform.rotation);
        Destroy(burst,3);
        if(capturedTarget != null)
        {
            ReleaseTarget();
        }
    }

    IEnumerator ColliderDelay()
    {
        yield return new WaitForSeconds(colliderDelayTime);
        this.GetComponent<Collider2D>().enabled = true;
    }


}
