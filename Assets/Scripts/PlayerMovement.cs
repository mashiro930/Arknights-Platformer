using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("移动参数")]
    public float speed = 10f;
    public float crouchSpeedDivisor = 3f;

    float xVelocity;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        GroundMovement();
    }

    void GroundMovement()
    {
        xVelocity = Input.GetAxis("Horizontal");  // -1f, 1f
        
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);  

        FlipDirection();

        IsMove();
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
            anim.SetBool( name: "isMove", value: true);
            
        if (xVelocity > 0)
            anim.SetBool( name: "isMove", value: true);

        if (xVelocity == 0)
            anim.SetBool( name: "isMove", value: false); 
    }

}
