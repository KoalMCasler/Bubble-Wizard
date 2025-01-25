using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool isOnPatrol;
    public bool wasOnPatrol;
    public Transform point1;
    public Transform point2;
    public Transform targetPoint;
    public Rigidbody2D enemyRB;
    public float moveSpeed;
    public Animator enemyAnim;
    // Start is called before the first frame update
    void Start()
    {
        targetPoint = point1;
        enemyRB = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        enemyAnim.SetBool("isOnPatrol",isOnPatrol);
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnPatrol)
        {
            //Flip();
            Vector2 point = targetPoint.position - transform.position;
            if(targetPoint == point1)
            {
                enemyRB.velocity = new Vector2(moveSpeed, 0);
            }
            else
            {
                enemyRB.velocity = new Vector2(-moveSpeed, 0);
            }
            if(Vector2.Distance(transform.position, targetPoint.position) < 0.5f && targetPoint == point1)
            {
                Flip();
                targetPoint = point2;
            }
            if(Vector2.Distance(transform.position, targetPoint.position) < 0.5f && targetPoint == point2)
            {
                Flip();
                targetPoint = point1;
            }
        }
        else if(transform.position.x > point1.position.x || transform.position.x < point2.position.x)
        {
            isOnPatrol = false;
            enemyAnim.SetBool("isOnPatrol",isOnPatrol);
        }
    }
    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Platform") && wasOnPatrol && !isOnPatrol)
        {
            isOnPatrol = true;
            wasOnPatrol = false;
            enemyAnim.SetBool("isOnPatrol",isOnPatrol);
        }
    }


}
