using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMovement : MonoBehaviour { 
    private CharacterController controller; //colocar o player com o characterController
    private Animator animator;
    public Transform spider;
    public float speed = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;

    public Transform groundCheck;
    public Transform cam;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float turnSmothVelocity;
    bool isGround =true;
    bool checkGround;
    Vector3 velocity;

	private void Start()
	{
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update()
    {
        float running = speed;
        

        if(isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        animator.SetFloat("DirectionMagnetude", direction.magnitude);



        if (direction.magnitude >= 0.1f)
        {
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                        running = running * 2;
            }

                  Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * running * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGround)
        {
            checkGround = false;
            animator.Play("metarig|Luan-Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsGrounded",false);
            Invoke("Jump", 0.3f);
        }

        if (checkGround == true) {
            animator.SetBool("IsGrounded", isGround);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
    }
    void Jump() {
        checkGround = true;
    }
}
