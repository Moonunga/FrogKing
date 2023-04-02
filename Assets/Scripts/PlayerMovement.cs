using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    [SerializeField] private LayerMask jumpableGround;
    private float dirX = 0f;
    [SerializeField]private float moveSpeed = 7f;
    [SerializeField] private float jumpforce = 14f;

    private enum MovementState { idle, run, jump, fall }

    // Start is called before the first frame update
    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2 (dirX * moveSpeed, rb.velocity.y);

        
        if (Input.GetButtonDown("Jump") && IsGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.run;
            spriteRenderer.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.run;
            spriteRenderer.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.fall;
        }

        animator.SetInteger("state", (int)state);

    }

    private bool IsGround()
    {
       return Physics2D.BoxCast(coll.bounds.center , coll.bounds.size , 0f ,Vector2.down , 0.1f, jumpableGround);
    }

}
