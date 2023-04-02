using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class npcControll : MonoBehaviour
{
    //npc talk---------------------------------------
    [SerializeField] private GameObject talkingUI;
    [SerializeField] private Text nameText;
    [SerializeField] private Text conversationText;
    [SerializeField] private Image faceimage;

    [SerializeField] private string myname;
    [SerializeField] private string prompt;
    [SerializeField] private Sprite myface;

    bool startTalking = false;
    bool onoffTalking = false;

    // npc movement-----------------------------------
    [SerializeField] private bool moveable = false;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed = 7f;

    public float delay = 1f;
    float timer;
    private int randomWalk = 0;
    private enum MovementState { idle, run, jump, fall }
    //---------------------------------------------------



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startTalking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startTalking = false;
            talkingUI.SetActive(false);
        }
    }



    // Start is called before the first frame update
    void Start()
    {

        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && startTalking)
        {
            faceimage.sprite = myface;
            nameText.text = myname;
            conversationText.text = prompt;
            onoffTalking = !onoffTalking;
            talkingUI.SetActive(onoffTalking);
        }

        if (moveable)
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                randomwalkTime();
                timer -= delay;
            }

            rb.velocity = new Vector2(randomWalk * moveSpeed, rb.velocity.y);

            UpdateAnimation();
        }

    }

    void randomwalkTime()
    { 
        randomWalk = Random.Range(-1, 2);
        Debug.Log(randomWalk);
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (randomWalk > 0f)
        {
            state = MovementState.run;
            spriteRenderer.flipX = false;
        }
        else if (randomWalk < 0f)
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
}
