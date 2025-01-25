using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Rigidbody2D RB2D;
    private Animator playerAnim;
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public float shotForce;
    public bool isGrounded;
    public float aimAngleMod;
    public float shotDelay;
    public float jumpTime;
    private bool isAttacking;
    public GameObject bubbleSpell;
    public Transform attackPoint;
    public bool isFacingLeft;
    public Vector2 moveDirection;
    public GameObject activeBubble;
    public Rigidbody2D bubbleRB;
    private Vector2 standingOffset;
    private Vector2 jumpOffset;
    public PlayerInput playerInput;
    public Transform jumpParticlePosition;
    public GameObject jumpParticleEffect;
    public float particleDecayRate;

    // Start is called before the first frame update
    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        standingOffset = GetComponent<Collider2D>().offset;
        playerInput = GetComponent<PlayerInput>();
        jumpOffset = new Vector2(standingOffset.x,-0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking)
        {
            playerInput.enabled = false;
        }
        else
        {
            playerInput.enabled = true;
        }
    }

    void FixedUpdate()
    {
        RB2D.velocity = new Vector2(moveDirection.x * moveSpeed, RB2D.velocity.y);
        if(isFacingLeft)
        {
            transform.rotation = new Quaternion(0,180,0,1);
        }
        else
        {
            transform.rotation = new Quaternion(0,0,0,1);
        }
    }

    void OnMove(InputValue movementValue)
    {
        if(gameManager.gameState == GameManager.GameState.Gameplay)
        {
            moveDirection = movementValue.Get<Vector2>();
            if(moveDirection.x < 0)
            {
                isFacingLeft = true;
            }
            else if(moveDirection.x > 0)
            {
                isFacingLeft = false;
            }
            if(moveDirection.x != 0)
            {
                playerAnim.SetBool("IsMoveing", true);
            }
            else
            {
                playerAnim.SetBool("IsMoveing", false);
            }

        }
    }

    void OnFire()
    {
        if(isGrounded)
        {
            isAttacking = true;
            //Debug.Log("Bubble Triggered");
            playerAnim.SetTrigger("Attack");
            StartCoroutine("CastBubble");
        }
    }

    private IEnumerator CastBubble()
    {
        yield return new WaitForSeconds(shotDelay);
        if(activeBubble == null)
        {
            activeBubble = Instantiate(bubbleSpell,attackPoint.position,attackPoint.rotation);
            activeBubble.GetComponent<BubbleSpell>().isFacingLeft = isFacingLeft;
            bubbleRB = activeBubble.GetComponent<Rigidbody2D>();
            if(isFacingLeft)
            {
                bubbleRB.AddForce((Vector2.left + (Vector2.down/aimAngleMod)) * shotForce);
            }
            else
            {
                bubbleRB.AddForce((Vector2.right + (Vector2.down/aimAngleMod)) * shotForce);
            }
        }
        else
        {
            Destroy(activeBubble);
            activeBubble = Instantiate(bubbleSpell,attackPoint.position,attackPoint.rotation);
            activeBubble.GetComponent<BubbleSpell>().isFacingLeft = isFacingLeft;
            bubbleRB = activeBubble.GetComponent<Rigidbody2D>();
            if(isFacingLeft)
            {
                bubbleRB.AddForce((Vector2.left + (Vector2.down/aimAngleMod)) * shotForce);
            }
            else
            {
                bubbleRB.AddForce((Vector2.right + (Vector2.down/aimAngleMod)) * shotForce);
            }
        }
        isAttacking = false;
    }

    void OnJump()
    {
        if(isGrounded)
        {
            playerAnim.SetTrigger("Jump");
            //Debug.Log("Jump Triggered");
            RB2D.AddForce(transform.up * jumpForce);
            GameObject jumpParticles = Instantiate(jumpParticleEffect,jumpParticlePosition);
            Destroy(jumpParticles,particleDecayRate);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Platform") || col.gameObject.CompareTag("Moveable"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Platform") || col.gameObject.CompareTag("Moveable"))
        {
            isGrounded = false;
        }
    }

    public void Bounce()
    {
        RB2D.AddForce(transform.up * jumpForce*1.25f);
        GameObject jumpParticles = Instantiate(jumpParticleEffect,jumpParticlePosition);
        Destroy(jumpParticles,particleDecayRate);
    }
}
