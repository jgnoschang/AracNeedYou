using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyController : MonoBehaviour
{


    private string turnInputAxis = "Horizontal";
    [Tooltip("Rate per seconds holding down input")]
    public float rotationRate = 0.10f, speedWalk;
    public TeiaControler teiaControler;
    public Camera cameraFrente;
    public Rigidbody playerRB;

   

    RaycastHit hit;
    //--

    //public CharacterController controller;
    //public float velocidade = 6;
    private void Update()
    {
        //- CONTROL MOUSE
        //CameraPrimeiraPessoa();
        //------------------------------------


        Ray raySelected = new Ray(playerRB.transform.position, -playerRB.transform.up);
        if (Physics.Raycast(raySelected, out hit, 2) || teiaControler.PodeLancar)
            if (Input.GetKeyDown(KeyCode.Space)) Jump();


        if (Physics.Raycast(raySelected, out hit, 1) || teiaControler.PodeLancar)
        {
            if (Input.GetKey(KeyCode.W))
                   playerRB.AddRelativeForce(Vector3.forward * (speedWalk * playerRB.mass));
            if (Input.GetKey(KeyCode.S))
                playerRB.AddRelativeForce(-Vector3.forward * (speedWalk * playerRB.mass));
        }


        //------------------------------------
        //Turn player - CONTROL TECLADO
        ////////float turnAxis = Input.GetAxis(turnInputAxis);
        ////////ApplyInput(turnAxis);
        //------------------------------------


    }//end update

    //Void for Turn Player
    //private void ApplyInput(float turnInput)
    //{
    //    Turn(turnInput);
    //}
    ////

    ////Void for turn player
    //private void Turn(float input)
    //{
    //    transform.Rotate(0, input * rotationRate * Time.deltaTime, 0);
    //}


    public int contagemPulo;
    float power;
    private void Jump()
    {
        //teiaControler.AudioManager.playAudio(3);

        if (teiaControler.PodeLancar)
        {  
            if(contagemPulo==0)
                power = 600;
            else if (contagemPulo < 2)
            {
              contagemPulo++;
              power -= 400;
            }
            print("contagemPulo" + contagemPulo);
        }
        else
            power =600;

        playerRB.AddRelativeForce(Vector3.up * (power * playerRB.mass));
    }
    //
















    float rotacaoX = 0.0f, rotacaoY = 0.0f;
    //void CameraPrimeiraPessoa()
    //{

    //    rotacaoX += Input.GetAxis("Mouse X") * 4.2f;
    //    rotacaoY += Input.GetAxis("Mouse Y") * 4.2f;
    //    rotacaoX = ClampAngleFPS(rotacaoX, -360, 360);
    //    rotacaoY = ClampAngleFPS(rotacaoY, -55, 55);
    //    Quaternion xQuaternion = Quaternion.AngleAxis(rotacaoX, Vector3.up);
    //    Quaternion yQuaternion = Quaternion.AngleAxis(rotacaoY, -Vector3.right);
    //    Quaternion rotacFinal = Quaternion.identity * xQuaternion * yQuaternion;

    //    transform.rotation = Quaternion.Lerp(transform.rotation, rotacFinal, Time.deltaTime * 7.0f);


    //}

    float ClampAngleFPS(float angulo, float min, float max)
    {
        if (angulo < -360)
        {
            angulo += 360;
        }
        if (angulo > 360)
        {
            angulo -= 360;
        }
        return Mathf.Clamp(angulo, min, max);
    }

}//end script