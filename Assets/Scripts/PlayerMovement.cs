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
    public float jumpVelocity_2 = 20f;
    public float jumpHoldForce = 2f;
    public float jumpHoldDuration = 0.1f;
    public int jumpCount = 2;

    public float jumpTime;
    public float leaveTime;

    [Header("冲刺参数")]
    public float dashVelocity = 30f;
    public float dashDuration = 0.3f;
    public float dashCoolDownDuration = 1f;
    public int dashCount = 1;

    public float dashTime;
    public float dashCoolDownTime;

    [Header("状态")]
    private bool isOnGround = false;
    private bool isMove = false;
    private bool isJump = false;
    private bool isDash = false;

    [Header("环境检测")]
    public float leftFootOffsetX = -0.75f;
    public float rightFootOffsetX = 0.55f;
    public float footOffsetY = -2.408f;
    public float groundDistance = 0.1f;

    public LayerMask groundLayer;

    //物理运动
    float xVelocity;

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool dashPressed;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (Input.GetButtonDown("Dash"))
            dashPressed = true;
        
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        jumpHeld = Input.GetButton("Jump");
        

    }

    private void FixedUpdate()
    {
        Dash();
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
        
    }

    void PhysicsCheck()
    {
        RaycastHit2D leftcheck = Raycast(new Vector2(leftFootOffsetX, footOffsetY) * transform .localScale, Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightcheck = Raycast(new Vector2(rightFootOffsetX, footOffsetY) * transform.localScale, Vector2.down, groundDistance, groundLayer);

        if (leftcheck || rightcheck)
        //if (coll.IsTouchingLayers(groundLayer))
        {
            isOnGround = true;
            jumpCount = 2;
            dashCount = 1;
        }
        else
            isOnGround = false;
    }

    void GroundMovement()
    {
        if (!isDash)
        {
            xVelocity = Input.GetAxis("Horizontal");  // -1f, 1f
        
            rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);  

            FlipDirection();

            IsMove();
        }
        
    }

    void MidAirMovement()
    {
        if (jumpPressed && !isJump && jumpCount == 2 && isOnGround)
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

        else if (jumpPressed && (jumpCount == 1 ||(jumpCount == 2 && !isOnGround)))
        {
            isJump = true;

            rb.velocity = new Vector2(xVelocity * speed, jumpVelocity_2);

            jumpPressed = false;
            jumpCount = jumpCount - 2;
        }
       
    }

    void Dash()
    {
        if (dashPressed && !isDash && dashCoolDownTime < Time.time && dashCount == 1)
        {
            isDash = true;

            dashTime = Time.time + dashDuration;
            dashCoolDownTime = Time.time + dashCoolDownDuration;

            rb.velocity = new Vector2(dashVelocity, 0f) * transform.localScale;
            rb.AddForce(new Vector2(0f, 50f), ForceMode2D.Force);

            dashCount--;
        }

        else if (isDash)
        {
            if (dashTime < Time.time)
                isDash = false;

            dashPressed = false;
        }

        IsDash();
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
        if (!isDash)
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

    void IsDash()
    {
        anim.SetBool(name: "isDash", value: isDash);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float distance, LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, distance, layer);

        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, rayDirection * distance, color);

        return hit;
    }

}
