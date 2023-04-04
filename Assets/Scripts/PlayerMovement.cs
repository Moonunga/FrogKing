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

    //--- Get hit--------
    [SerializeField] private float gethitForce;
    bool isgethit;

    //--------------double jump
    [SerializeField] int jumpCount = 0;
    bool isdoublejump = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("doublejumpman"))
        {
            isdoublejump = true;
        }
    }

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
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);


        if (Input.GetButtonDown("Jump") && (IsGround() || (jumpCount < 1 && isdoublejump) ) ) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            ++jumpCount;
            if (IsGround())
                jumpCount = 0;
        }

        UpdateAnimation();

    }

 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            GetHit(collision.transform.position);
            rb.velocity = new Vector2( rb.velocity.x, jumpforce);

        }
    }

    private void GetHit(Vector2 pos)
    {
        if (!isgethit)
        {
            isgethit = true;
            
            float x = transform.position.x - pos.x;
            if (x < 0)
                x = 1;
            else
                x = -1;

            StartCoroutine(HitRoutine());
            StartCoroutine(Knockback(x));
            StartCoroutine(alphablink());
        }
       
    }

    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(0.7f);
        isgethit = false;
    }

    IEnumerator alphablink()
    {
        while (isgethit)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    IEnumerator Knockback(float dir)
    {
        float knockTime = 0;
        while(knockTime<0.2f)
        {
            if (transform.rotation.y == 0)
                transform.Translate(Vector2.left * gethitForce * Time.deltaTime * dir);
            else
                transform.Translate(Vector2.left * gethitForce * Time.deltaTime *-1f* dir);

            knockTime += Time.deltaTime;
            yield return null;
        }
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

        if (isgethit)
        {
            animator.SetBool("get_hit", true);
        }
        else
        {
            animator.SetBool("get_hit", false);
        }


        animator.SetInteger("state", (int)state);

    }

    private bool IsGround()
    {
       return Physics2D.BoxCast(coll.bounds.center , coll.bounds.size , 0f ,Vector2.down , 0.1f, jumpableGround);
    }

}
