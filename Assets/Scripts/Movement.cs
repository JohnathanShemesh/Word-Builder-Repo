using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;

    public Transform raycastOrigin;
    public float rayLength = 0.5f;
    public LayerMask groundLayer;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (raycastOrigin == null)
            Debug.LogError("raycastOrigin לא הוגדר!");
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, Vector2.down, rayLength, groundLayer);

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
    }

    private void OnDrawGizmos()
    {
        if (raycastOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + Vector3.down * rayLength);
        }
    }


}
