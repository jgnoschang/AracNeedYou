using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public TeiaControler teiaControler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {


        print("colision:" + collision.transform.gameObject);
        if (collision.transform.gameObject.tag == "Player") {
            //teiaControler.AudioManager.playAudio(4);
            Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider collider)
    {
        print("collider:" + collider.transform.gameObject);
        if (collider.transform.gameObject.tag == "Player")
        {
            //teiaControler.AudioManager.playAudio(4);
            Destroy(gameObject);
        }
    }


}
