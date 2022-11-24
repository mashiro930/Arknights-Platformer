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
    public float dashVelocity = 25f;
    public float dashDuration = 0.35f;
    public float dashCoolDownDuration = 1f;
    public int dashCount = 1;

    public float dashTime;
    public float dashCoolDownTime;

    [Header("攻击参数")]
    public float attackDuration = 0.8f;
    public float attackCoolDownDuration = 1f;

    public float attackTime;
    public float responseTime;
    public float attackResponseTime;
    public float attackCoolDownTime;

    public bool attackEnable = true;

    [Header("状态")]
    private bool isOnGround = false;
    private bool isMove = false;
    private bool isJump = false;
    private bool isDash = false;
    private bool isAttack = false;

    [Header("环境检测")]
    public float leftFootOffsetX = -0.75f;
    public float rightFootOffsetX = 0.55f;
    public float footOffsetY = -2.408f;
    public float groundDistance = 0.1f;

    public LayerMask groundLayer;

    [Header("攻击范围")]
    public GameObject attackArea1;
    public GameObject attackArea2;

    //物理运动
    float xVelocity;

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool dashPressed;
    bool attackPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Dash") && dashCount > 0 && !isAttack)
            dashPressed = true;
        
        if (Input.GetButtonDown("Jump") && jumpCount > 0 && !isDash)
            jumpPressed = true;
        jumpHeld = Input.GetButton("Jump");

        if (Input.GetButtonDown("Attack") && !isDash)
            attackPressed = true;
        
    }

    private void FixedUpdate()
    {
        Dash();
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
        Attack();
        IsJump();
    }
    
    void PhysicsCheck()  //环境检测
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

    void GroundMovement()  //地面移动
    {
        if (!isDash && !isAttack)
        {
            xVelocity = Input.GetAxis("Horizontal");  // -1f, 1f
        
            rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);  

            FlipDirection();

            IsMove();
        }
        
    }

    void MidAirMovement()  //跳跃
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
            {
                isJump = false;
                jumpPressed = false;
            }

            if (leaveTime < Time.time && jumpCount == 2)
                jumpCount--;
            
        }

        else if (jumpPressed && (jumpCount == 1 ||(jumpCount == 2 && !isOnGround)))
        {
            isJump = true;

            rb.velocity = new Vector2(xVelocity * speed, jumpVelocity_2);

            jumpPressed = false;
            jumpCount -= 2;
        }
        
    }

    void Dash()  //冲刺
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
            {
                isDash = false;
                dashPressed = false;
            }
        }

        IsDash();
    }

    void Attack()  //攻击
    {
        if (attackPressed && !isAttack && attackCoolDownTime < Time.time)
        {
            isAttack = true;

            attackTime = Time.time + attackDuration;
            attackResponseTime = Time.time + responseTime;
            attackCoolDownTime = Time.time + attackCoolDownDuration;

            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        else if (isAttack)
        {
            if (attackResponseTime < Time.time && attackEnable)
                
            {   if (transform.localScale.x > 0)
                {
                    var attack = AttackRight();
                    attack.transform.parent = rb.transform;
                    attack.transform.localPosition = new Vector2(0, 0);
                    attackEnable = false;
                }
                if (transform.localScale.x < 0)
                {
                    var attack = AttackLeft();
                    attack.transform.parent = rb.transform;
                    attack.transform.localPosition = new Vector2(0, 0);
                    attackEnable = false;
                }

            }


            if (attackTime < Time.time)
            {
                isAttack = false;
                attackPressed = false;
                attackEnable = true;
            }
        }

        IsAttack();
    }

    void FlipDirection()  //转向
    {
        if (xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);
            
        if (xVelocity > 0)
            transform.localScale = new Vector2(1, 1);
    }

    void IsMove()  //移动动画
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

    void IsDash()  //冲刺动画
    {
        anim.SetBool(name: "isDash", value: isDash);
    }

    void IsAttack()  //攻击动画
    {
        anim.SetBool(name: "isAttack", value: isAttack);
    }

    void IsJump()  //攻击动画
    {
        anim.SetBool(name: "isOnGround", value: isOnGround);
    }

    public GameObject AttackRight()
    {
        GameObject area = GameObject.Instantiate(attackArea1);

        return area;
    }

    public GameObject AttackLeft()
    {
        GameObject area = GameObject.Instantiate(attackArea2);

        return area;
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
