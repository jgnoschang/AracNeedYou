using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Rigidbody RB;
    public float jumpForce;
    /*
	 * A base:
	 https://medium.com/ironequal/unity-character-controller-vs-rigidbody-a1e243591483
	*/

    private CharacterController _controller;
	public float speed = 5.0f;

    void Start()
    {
		_controller = GetComponent<CharacterController>();
    }

    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection;


    void Update()
    {
       
       moveDirection.y -= gravity * Time.deltaTime;
        
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, Input.GetAxis("Vertical"));
        _controller.Move(move * Time.deltaTime * speed);//movimentar o player
       // _controller.Move(Physics.gravity * Time.deltaTime);//Dar fisica ao player - gravidade;

        //--------------------------------------

        if (_controller.isGrounded && Input.GetButtonDown("Jump"))
             moveDirection.y = jumpSpeed;

        //--------------------------------------

        //virar a frente do player para a direção desejada
        if (move != Vector3.zero) { 
            transform.forward = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        // _controller.Move(moveDirection * Time.deltaTime);

    }
}
