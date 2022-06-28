using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teia : MonoBehaviour
{
    public float timer;
    public bool activeTimer;
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartProcessWebDestruction()
    {
        
        checkWebOriginPosition();
        activeTimer = true;
    }

    private void FixedUpdate()
    {
        if (activeTimer)
        {
            timer += 1 * Time.deltaTime;

            if (timer > 7)
            {
                DestroyWeb();
            }
        }

        TeiaControllerLastFrame = webOrigin.transform.position;
        lineRenderer.SetPosition(0, TeiaControllerLastFrame);


    }

  
    public GameObject webOrigin;
    public Vector3 TeiaControllerLastFrame;
    void checkWebOriginPosition()
    {
        TeiaControllerLastFrame = GameObject.FindGameObjectWithTag("TeiaController").transform.position;
        webOrigin.transform.position = TeiaControllerLastFrame;
        webOrigin.AddComponent<Rigidbody>();

    }




    void DestroyWeb()
    {
        Destroy(this.gameObject);
    }

}
