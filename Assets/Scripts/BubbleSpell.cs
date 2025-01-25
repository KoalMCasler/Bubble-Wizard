using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleSpell : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        bubbleRb = this.GetComponent<Rigidbody2D>();
        hasExpanded = false;
        player = FindObjectOfType<PlayerController>();
        this.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("ColliderDelay");
        transform.localScale = Vector3.one;
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
    }

    void TrapTarget()
    {
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
        GameObject burst = Instantiate(popParticles,transform.position,transform.rotation);
        Destroy(burst,1);
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
