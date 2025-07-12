using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    public Animator animator;
    public Transform raycastOrigin;
    public BoxCollider2D boxCollider;
    public float rayLength = 0.5f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {            
        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (raycastOrigin == null)
            Debug.LogError("raycastOrigin לא הוגדר!");
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed",Mathf.Abs(rb.velocity.x));
        if(rb.velocity.x < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        Vector2 boxSize = new Vector2(boxCollider.size.x * 0.9f, 0.05f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            Debug.Log("Hit ground: " + hit.collider.name);
            isGrounded = true;
        }
        else
        {
            Debug.Log("Not grounded");
            isGrounded = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        bool isCrouching = isGrounded && Input.GetKey(KeyCode.S);
        if (isGrounded && Input.GetKey(KeyCode.S))
        {
            Crouch(isCrouching);
            Debug.Log("Crouching"); // בדיקה
            animator.SetBool("IsCrouching", true);
        }
        else 
        {
            Crouch(isCrouching);
            Debug.Log("Not Crouching"); // בדיקה
            animator.SetBool("IsCrouching", false);
        }
    }
    
    public void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            // גודל חדש קטן יותר (לדוגמה חצי גובה)
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y / 2f);
            // הורדת הקוליידר מעט למטה (כדי שיתאים לגוף התכופף)
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y / 4f);
        }
        else
        {
            // מחזירים לגודל ומיקום המקורי
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(boxCollider.size.x * 0.9f, 0.05f);
            Gizmos.DrawWireCube(boxCollider.bounds.center + Vector3.down * rayLength, boxSize);
        }
    }



}
