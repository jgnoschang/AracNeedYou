using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPosition : MonoBehaviour
{

    // Update is called once per frame
    private void FixedUpdate()
    {
        RaycastHit hit;
        if ((Physics.Raycast(new Vector3 (transform.position.x, transform.position.y+1, transform.position.z), -Vector3.up, out hit, 1.2f)))
        {

            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

        }
    }
}
