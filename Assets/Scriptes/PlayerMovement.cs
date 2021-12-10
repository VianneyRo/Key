using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Vitesse de déplacement
    public float moveSpeed;
    // Force du saut
    public float jumpForce;

    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isChangingGravity;
    [SerializeField] private bool isTop;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position);
        
        // Déplacement sur l'axe horizontale
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        // Saut
        if (Input.GetButton("Jump") && isGrounded)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        if (!isTop)
        {
            // Go up
            if (Input.GetButton("Jump") && Input.GetAxis("Vertical") > 0)
            {
                Debug.Log("Change gravity");
                isChangingGravity = true;
            }
        }
        else
        {
            // Go down
            if (Input.GetButton("Jump") && Input.GetAxis("Vertical") < 0)
            {
                Debug.Log("Change gravity");
                isChangingGravity = true;
            }
        }

        // Tourne le personnage
        Flip(rb.velocity.x);
        MovePlayer(horizontalMovement);
        
        // Valeur absolue de rb.velocity.x
        float characterVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", characterVelocity);
    }

    void MovePlayer(float horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
        
        // Lissage du déplacement
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);

        if (isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }

        if (isChangingGravity)
        {
            rb.gravityScale *= -1;
            isChangingGravity = false;
            RotationGravity();
        }

    }

    void RotationGravity()
    {
        if (!isTop)
        {
            transform.eulerAngles = new Vector3(0, 0, 180f);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }

        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        isTop = !isTop;
    }

    void Flip(float velocityX)
    {
        if (velocityX > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (velocityX < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
