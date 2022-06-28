using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeiaControler : MonoBehaviour
{
    public GameObject player;
    public GameObject Gancho;
    public GameObject CameraDi;
    public bool PodeLancar;
    public float ForcaGancho, SpeedCarretel;
    public GameObject origem;
    public int tipoGancho;
    //public audioManager AudioManager;

    void Start()
    {
        CameraDi = GameObject.FindGameObjectWithTag("MainCamera");
        //AudioManager.playAudio(0);
        Gancho.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
     //   Cursor.lockState = CursorLockMode.Confined;
    }

   public LayerMask nameLayer;
    RaycastHit hit;
    GameObject hitGameOject;
    void Update()
    {
        if (PodeLancar)
        {
            if (tipoGancho == 1)
            {
                player.GetComponent<SpringJoint>().spring = 0;
                player.GetComponent<SpringJoint>().connectedBody = null;
                Gancho.GetComponent<SpringJoint>().connectedBody = player.GetComponent<Rigidbody>();
                Gancho.GetComponent<SpringJoint>().spring = ForcaGancho;

                Gancho.GetComponent<LineRenderer>().SetPosition(0, origem.transform.position);
                Gancho.GetComponent<LineRenderer>().SetPosition(1, Gancho.transform.position);
            }
            else if (tipoGancho == 2)
            {
                Gancho.GetComponent<SpringJoint>().spring = 0;
                Gancho.GetComponent<SpringJoint>().connectedBody = null;
                player.GetComponent<SpringJoint>().connectedBody = hitGameOject.GetComponent<Rigidbody>();
                player.GetComponent<SpringJoint>().spring = ForcaGancho;

                Gancho.GetComponent<LineRenderer>().SetPosition(0, origem.transform.position);
                Gancho.GetComponent<LineRenderer>().SetPosition(1, hitGameOject.transform.position);
            }
            else
            {
                PodeLancar = false;
                Gancho.SetActive(false);
                tipoGancho = 0;
                player.GetComponent<SpringJoint>().spring = 0;
                Gancho.GetComponent<SpringJoint>().spring = 0;
                player.GetComponent<SpringJoint>().connectedBody = null;
                Gancho.GetComponent<SpringJoint>().connectedBody = null;
            }
        }


       
        //Debug.DrawRay(origem.transform.position, origem.transform.forward * 300, Color.yellow);
     

        print("tipoGancho:" + tipoGancho);
        if (Input.GetMouseButtonUp(2))
        {
            player.GetComponent<SpringJoint>().spring = 0;
            Gancho.GetComponent<SpringJoint>().spring = 0;
            player.GetComponent<SpringJoint>().connectedBody = null;
            Gancho.GetComponent<SpringJoint>().connectedBody = null;
        }

        


        if (Input.GetMouseButtonDown(0))
        {


            Ray rayforward = new Ray(origem.transform.position, CameraDi.transform.forward);
            Ray raySelected = rayforward;


            if (Physics.Raycast(raySelected, out hit, 300))
            {
                if (hit.transform.tag == "Untagged")
                { 
                    tipoGancho = 1;//puxaPlayer
                    //AudioManager.playAudio(1);

                    Gancho.transform.position = hit.point;
                    PodeLancar = true;
                    Gancho.SetActive(true);

                }                    
                else if (hit.transform.tag == "puxavel")
                {
                    tipoGancho = 2;//puxaBloco
                    hitGameOject = hit.transform.gameObject;
                    //AudioManager.playAudio(2);

                    Gancho.transform.position = hit.point;
                    PodeLancar = true;
                    Gancho.SetActive(true);
                }
                else
                {
                    tipoGancho = 0;
                    PodeLancar = false;
                    player.GetComponent<SpringJoint>().spring = 0;
                    Gancho.GetComponent<SpringJoint>().spring = 0;
                    player.GetComponent<SpringJoint>().connectedBody = null;
                    Gancho.GetComponent<SpringJoint>().connectedBody = null;
                }
            }
            else
            {
                tipoGancho = 0;
                PodeLancar = false;
                player.GetComponent<SpringJoint>().spring = 0;
                Gancho.GetComponent<SpringJoint>().spring = 0;
                player.GetComponent<SpringJoint>().connectedBody = null;
                Gancho.GetComponent<SpringJoint>().connectedBody = null;
            }

          
        }else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            PodeLancar = false;
            Gancho.SetActive(false);
            ForcaGancho = 15;
            //--

            player.GetComponent<SpringJoint>().spring = 0;
            Gancho.GetComponent<SpringJoint>().spring = 0;
            player.GetComponent<SpringJoint>().connectedBody = null;
            Gancho.GetComponent<SpringJoint>().connectedBody = null;
        }
        //----


        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            ForcaGancho += Input.GetAxis("Mouse ScrollWheel")*SpeedCarretel;
        }

      
        if (Input.GetKey(KeyCode.M))
        {
            if (Cursor.visible)
                Cursor.visible = false;
            else
                Cursor.visible = true;
        }

        if (Input.GetKey(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;


        if (Input.GetKey(KeyCode.P) )
        {
            //  curvedRaycast(10, origem.transform.position, new Ray(origem.transform.position, origem.transform.forward), 6);
        }


    }





    //void curvedRaycast(int iterations, Vector3 startPos, Ray ray, int velocity)
    //{
    //    RaycastHit hit;
    //    Vector3 pos = startPos;
    //    var slicedGravity = Physics.gravity.y / iterations / velocity;
    //    Ray ray2 = new Ray(ray.origin, ray.direction);
    //    print(slicedGravity);



    //    for (int i = 0; i < iterations; i++)
    //    {
    //        if (Physics.Raycast(pos, ray2.direction * velocity, out hit, velocity))
    //        {
    //            Debug.DrawRay(pos, ray2.direction * hit.distance, Color.green);
    //            if (hit.transform.tag == "Permeable")
    //            {
    //                Debug.DrawRay(pos, ray2.direction * velocity, Color.green);
    //                pos += ray2.direction * velocity;
    //                ray2 = new Ray(ray2.origin, ray2.direction + new Vector3(0, slicedGravity, 0));
                
    //            }
    //            else
    //            {
    //                return;
    //            }
    //        }
    //        Debug.DrawRay(pos, ray2.direction * velocity, Color.cyan);
    //        pos += ray2.direction * velocity;
    //        ray2 = new Ray(ray2.origin, ray2.direction + new Vector3(0, slicedGravity, 0));
    //    }
   
    //}


















}
