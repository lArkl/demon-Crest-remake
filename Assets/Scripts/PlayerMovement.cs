using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed, jumpFactor, fireDelay;
    
    public bool onGround, onWall, onFly;
    public GameObject bulletPrefab;
    bool jumpPressed, firePressed;
    float fireNext;

    Rigidbody2D _refRigidBody;
    Animator _refAnim;

    float horizontalmov;

    // Start is called before the first frame update
    void Start()
    {
        _refRigidBody = gameObject.GetComponent<Rigidbody2D>();
        _refAnim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!onWall && !firePressed)
            Move(Time.deltaTime);

        //Salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        //Ataque
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time > fireNext )
        {
            firePressed = true;
            FireAction();
            fireNext = Time.time + fireDelay;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)){
            firePressed = false;
        }

    }

    private void FixedUpdate()
    {
        if (jumpPressed)
        {
            JumpAction(Time.fixedDeltaTime);
            jumpPressed = false;
        }

        //if(_refRigidBody.velocity.y < 0.0001 && !Input.GetKey(KeyCode.DownArrow))
        //{
        //    Debug.Log(_refRigidBody.velocity.y);
        //    _refAnim.SetTrigger("falling");
        //}

    }


    void Move(float delta)
    {
        horizontalmov = Input.GetAxis("Horizontal");
        _refRigidBody.position += new Vector2(horizontalmov * movespeed, 0) * delta;

        if (horizontalmov != 0)
        {
            _refRigidBody.transform.right = new Vector3(horizontalmov, 0);
            _refAnim.SetBool("walking", true);
        }else
            _refAnim.SetBool("walking", false);

    }

    void StopFall()
    {
        _refRigidBody.gravityScale = 0;
        _refRigidBody.velocity = Vector2.zero;
    }


    void WallCling()
    {
        StopFall();
        onWall = true;
        _refAnim.SetTrigger("onwall");
    }

    void Hover()
    {
        StopFall();
        _refAnim.SetTrigger("hovering");
    }


    void KeepFall()
    {
        _refRigidBody.gravityScale = 1;
        _refAnim.SetTrigger("falling");
    }

    void JumpAction(float delta)
    {
        if(onGround) { // Salta
            _refAnim.SetTrigger("jumping");
            _refRigidBody.AddForce(new Vector2(0, 1) * jumpFactor * delta, ForceMode2D.Impulse);
        }

        else if(onWall) // Se desengancha de la pared
        {
            KeepFall();
            onFly = false;
            onWall = false;
        }

        else // Vuela o deja de volar
        {
            onFly = !onFly;
            if (onFly)
            {
                Hover();
            }
            else
            {
                KeepFall();
            }
        }
    }

    void FireAction()
    {
        _refAnim.SetTrigger("firing");
        Instantiate(bulletPrefab, new Vector2(transform.position.x - transform.right.x/2, transform.position.y), 
            Quaternion.identity).transform.right = transform.right;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground")){
            onGround = true;
            _refAnim.SetBool("onground", onGround);
        }

        if (collision.collider.CompareTag("Wall") && !onGround && horizontalmov != 0)
        {
            //Se engancha a la pared
            WallCling();
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            onGround = false;
            _refAnim.SetBool("onground", onGround);
        }

    }
}
