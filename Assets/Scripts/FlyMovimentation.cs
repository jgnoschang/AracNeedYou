using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FlyMovimentation : MonoBehaviour
{

    public float speed;
    public int disEntreRot;

    private Vector3 atualPos, atualTarget;

    public Vector2 groundDistanceMinMax;

    public Vector2 animSpeedMinMax;

    private NavMeshAgent nav;

    public float targetHeight;

    public float disRaycast;

    public bool getUp;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        targetHeight = Random.Range(groundDistanceMinMax.x, groundDistanceMinMax.y);
        //getUp = (targetHeight > nav.baseOffset) ? true : false;
        atualTarget = RandomNavmeshLocation(0.5f);
        nav.SetDestination(atualTarget);

    }
    void FixedUpdate()
    {
        atualPos = new Vector3 (transform.position.x, transform.position.y-nav.baseOffset, transform.position.z);
        float step = speed * Time.fixedDeltaTime;

        
        if (distanceFromTarget(atualPos, atualTarget) < nav.stoppingDistance + 1) {

            atualTarget = RandomNavmeshLocation(disEntreRot);

            nav.SetDestination(atualTarget);
            
            
            

        }
        if (!getUp)
        {
            if (nav.baseOffset <= targetHeight)
            {
                float offsetDistance;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up * disRaycast, out hit))
                {
                    offsetDistance = hit.distance;
                    Debug.DrawLine(transform.position, hit.point, Color.cyan);
                    targetHeight = hit.point.y + Random.Range(groundDistanceMinMax.x, groundDistanceMinMax.y);
                    getUp = (targetHeight > nav.baseOffset) ? true : false;
                }

            }
            else
            nav.baseOffset = Mathf.Lerp(nav.baseOffset, targetHeight - 0.5f, step);
        }
        else if (getUp){
            if (nav.baseOffset >= targetHeight)
            {
                float offsetDistance;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up * disRaycast, out hit))
                {
                    offsetDistance = hit.distance;
                    Debug.DrawLine(transform.position, hit.point, Color.cyan);
                    targetHeight = hit.point.y + Random.Range(groundDistanceMinMax.x, groundDistanceMinMax.y);
                    getUp = (targetHeight > nav.baseOffset) ? true : false;
                }

            }
            else
            nav.baseOffset = Mathf.Lerp(nav.baseOffset, targetHeight+0.5f, step);
        }
        
            


    }
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
    private float distanceFromTarget(Vector3 atual, Vector3 target) {

        return Vector3.Distance(atual, target);
    }


    // Select a new direction to fly in randomly
    
}