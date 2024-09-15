using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class RobberMovement : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;
    public GameObject pedestrianTarget;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }
/*agent.isActiveAndEnabled &&*/
    void Seek(Vector3 location)
    {

        agent.SetDestination(location);
    
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Pursue()
    {
        Vector3 targetDif = target.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed + target.GetComponent<Drive>().currentSpeed);
        if (target.GetComponent<Drive>().currentSpeed <= 0.01f)
        {
            Seek(target.transform.position);
            return;
        }
        Seek(target.transform.position + target.transform.forward * lookAhead * 5);

    }

    void Evade()
    {
        Vector3 targetDif = target.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed +12f /*target.GetComponent<Drive>().currentSpeed*/);
        if (12f <= 0.01f)
        {
            Flee(target.transform.position);
            return;
        }
        Flee(target.transform.position + target.transform.forward * lookAhead * 5);

    }

    Vector3 wonderTarget = Vector3.zero;

    void Wonder()
    {
        float wonderRadius = 10;
        float wonderDistance = 20;
        float wonderJitter = 5;

        wonderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wonderJitter, 0, Random.Range(-1.0f, 1.0f) * wonderJitter);
        wonderTarget.Normalize();

        wonderTarget = wonderTarget * wonderRadius;
        Vector3 targetLocal = wonderTarget + new Vector3(0, 0, wonderDistance);

        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        Seek(targetWorld);
    }

    GameObject[] getHidingPlaces()
    {
        GameObject[] hidingSpots;
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");
        return hidingSpots;
    }


    void Hide()
    {
        float closestDistance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        GameObject[] hidingSpots = getHidingPlaces();

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hideDirection = hidingSpots[i].transform.position - target.transform.position;
            Vector3 hidePosition = hidingSpots[i].transform.position + hideDirection.normalized * 5;

            if (Vector3.Distance(this.transform.position, hidePosition) < closestDistance)
            {
                chosenSpot = hidePosition;
                closestDistance = Vector3.Distance(this.transform.position, hidePosition);
            }
        }
        Seek(chosenSpot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target)
        {
            Time.timeScale = 0f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        float closestDistance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        GameObject[] hidingSpots = getHidingPlaces();
        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hideDirection = hidingSpots[i].transform.position - target.transform.position;
            Vector3 hidePosition = hidingSpots[i].transform.position + hideDirection.normalized * 5;

            if (Vector3.Distance(this.transform.position, hidePosition) < closestDistance)
            {
                chosenSpot = hidePosition;
                closestDistance = Vector3.Distance(this.transform.position, hidePosition);
            }
        }
        if(closestDistance<15){
            Hide();
        }if (closestDistance > 15 && Vector3.Distance(target.transform.position, this.transform.position) < 40){
            Evade();
        }else {
            Seek(pedestrianTarget.transform.position);
        }
    }
}