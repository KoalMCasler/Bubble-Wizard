using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private UIManager uIManager;
    public SoundManager soundManager;
    public Rigidbody2D RB2D;
    private Animator playerAnim;
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public float shotForce;
    public bool isGrounded;
    public float shotDelay;
    private bool isAttacking;
    public GameObject bubbleSpell;
    public Transform attackPoint;
    public bool isFacingLeft;
    public Vector2 moveDirection;
    public GameObject activeBubble;
    public Rigidbody2D bubbleRB;
    public PlayerInput playerInput;
    public Transform jumpParticlePosition;
    public GameObject jumpParticleEffect;
    public float particleDecayRate;
    public float knockbackForce;

    // Start is called before the first frame update
    void Start()
    {
        RB2D = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        uIManager = FindObjectOfType<UIManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking || gameManager.gameState != GameManager.GameState.GamePlay)
        {
            playerInput.enabled = false;
            moveDirection = Vector2.zero;
            playerAnim.SetBool("isMoveing", false);
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
        if(isGrounded && moveDirection.x != 0)
        {
            soundManager.PlayContinuesSFX(0);
        }
        else
        {
            soundManager.contSFXSource.Stop();
        }
    }

    /// <summary>
    /// Move input event
    /// </summary>
    /// <param name="movementValue"></param>
    void OnMove(InputValue movementValue)
    {
        if(gameManager.gameState == GameManager.GameState.GamePlay)
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
                playerAnim.SetBool("isMoveing", true);
            }
            else
            {
                playerAnim.SetBool("isMoveing", false);
            }

        }
    }

    /// <summary>
    /// Fire input event
    /// </summary>
    void OnFire()
    {
        if(isGrounded)
        {
            soundManager.PlaySFX(4);
            isAttacking = true;
            //Debug.Log("Bubble Triggered");
            playerAnim.SetTrigger("Attack");
            StartCoroutine("CastBubble");
        }
    }

    /// <summary>
    /// Throws out a bubble based on how your facing and destroys any active bubbles.
    /// </summary>
    /// <returns></returns>
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
                bubbleRB.AddForce(Vector2.left * shotForce);
            }
            else
            {
                bubbleRB.AddForce(Vector2.right * shotForce);
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
                bubbleRB.AddForce(Vector2.left * shotForce);
            }
            else
            {
                bubbleRB.AddForce(Vector2.right * shotForce);
            }
        }
        isAttacking = false;
    }

    /// <summary>
    /// Jump input event
    /// </summary>
    void OnJump()
    {
        if(isGrounded)
        {
            soundManager.contSFXSource.Stop();
            soundManager.PlaySFX(1);
            playerAnim.SetTrigger("Jump");
            //Debug.Log("Jump Triggered");
            RB2D.AddForce(transform.up * jumpForce);
            GameObject jumpParticles = Instantiate(jumpParticleEffect,jumpParticlePosition);
            Destroy(jumpParticles,particleDecayRate);
        }
    }

    /// <summary>
    /// Pause input event.
    /// </summary>
    void OnPause()
    {
        if(gameManager.gameState == GameManager.GameState.GamePlay)
        {
            gameManager.ChangeGameState("Pause");
            uIManager.SetUIPause();
            
        }
        else if(gameManager.gameState == GameManager.GameState.Paused)
        {
            UnPause();
        }
    }


    void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Platform") || col.gameObject.CompareTag("Moveable") && col.gameObject.transform.position.y < transform.position.y)
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

    /// <summary>
    /// Used to bounce player off of bubbles.
    /// </summary>
    public void Bounce()
    {
        RB2D.AddForce(transform.up * jumpForce*1.25f);
        GameObject jumpParticles = Instantiate(jumpParticleEffect,jumpParticlePosition);
        Destroy(jumpParticles,particleDecayRate);
    }

    /// <summary>
    /// UnPause function, used by pause menu. 
    /// </summary>
    public void UnPause()
    {
        gameManager.ChangeGameState("GamePlay");
        uIManager.SetUIHUD();
    }

}
