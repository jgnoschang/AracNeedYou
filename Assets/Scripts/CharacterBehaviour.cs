using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    private bool onGround;
    public GameObject GiantFoot;
    public Canvas Morte;

    private void Start()
    {
        onGround = false;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death" && other.gameObject.layer == 6) {
            onGround = true;
        }
        if (other.gameObject.layer == 4)
        {
            Death();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Death" && other.gameObject.layer == 6) {
            onGround = false;
        }

    }
    private void DeathOnGround () {
        if (onGround) {
            GiantFoot.SetActive( true);
            Death();
        }
        
    }
    private void Death() {
        GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        gameObject.AddComponent<Rigidbody>();
        Instantiate (Morte);
    }
}
