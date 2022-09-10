using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    [Header("移动参数")]
    public float speed = 10f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce_1 = 15f;
    public float jumpVelocity_2 = 15f;
    public float jumpHoldForce = 2f;
    public float jumpHoldDuration = 0.1f;
    public int jumpCount = 2;

    public float jumpTime;
    public float leaveTime;

    [Header("状态")]
    private bool isOnGround = false;
    private bool isMove = false;
    private bool isJump = false;

    [Header("环境检测")]
    public LayerMask groundLayer;

    //物理运动
    float xVelocity;

    //按键设置
    bool jumpPressed;
    bool jumpHeld;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        jumpHeld = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }
    void PhysicsCheck()
    {
        if (coll.IsTouchingLayers(groundLayer))
        {
            isOnGround = true;
            jumpCount = 2;
        }
        else isOnGround = false;
    }

    void GroundMovement()
    {
        xVelocity = Input.GetAxis("Horizontal");  // -1f, 1f
        
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);  

        FlipDirection();

        IsMove();
    }

    void MidAirMovement()
    {
        if (jumpPressed && !isJump && jumpCount == 2)
        {
            isOnGround = false;
            isJump = true;

            leaveTime = Time.time;
            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce_1), ForceMode2D.Impulse);

            jumpPressed = false;

        }

        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
            jumpPressed = false;
            if (leaveTime < Time.time && jumpCount == 2)
                jumpCount--;
            
        }

        else if (jumpPressed && jumpCount == 1)
        {
            isJump = true;

            rb.velocity = new Vector2(xVelocity * speed, jumpVelocity_2);

            jumpPressed = false;
            jumpCount--;
        }
       
    }

    void FlipDirection()
    {
        if (xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);
            
        if (xVelocity > 0)
            transform.localScale = new Vector2(1, 1);
    }

    void IsMove()
    {
        if (xVelocity < 0)
            isMove = true;

        if (xVelocity > 0)
            isMove = true;

        if (xVelocity == 0)
            isMove = false;

        anim.SetBool(name: "isMove", value: isMove);
    }

}
