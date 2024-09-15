/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Pedestrians : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;

    void Start(){
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Flee(Vector3 location){
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Evade(){
        Vector3 targetDif = target.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed + 12f/*target.GetComponent<Drive>().currentSpeed);
        if (/*target.GetComponent<Drive>().currentSpeed12f <= 0.01f){
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

        agent.SetDestination(targetWorld);
    }

    void Update()
    {
        if (Vector3.Distance(target.transform.position, this.transform.position) < 30)
        {
            agent.speed= 15f;
            Evade();
        }else{
            agent.speed= 3.5f;
            Wonder();
        }
    }
}*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Pedestrians : MonoBehaviour
{
    NavMeshAgent agent;
    public List<GameObject> targets = new List<GameObject>(); // List of targets

    Vector3 wonderTarget = Vector3.zero;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Evade()
    {
        if (targets.Count == 0) return; // No targets to evade from

        // Find the closest target
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject potentialTarget in targets)
        {
            float distance = Vector3.Distance(potentialTarget.transform.position, this.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = potentialTarget;
            }
        }

        if (closestTarget == null) return;

        Vector3 targetDif = closestTarget.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed + 12f); // You can replace 12f with the appropriate speed
        if (12f <= 0.01f) // Replace 12f with the actual speed value of the target if needed
        {
            Flee(closestTarget.transform.position);
            return;
        }
        Flee(closestTarget.transform.position + closestTarget.transform.forward * lookAhead * 5);
    }

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

        agent.SetDestination(targetWorld);
    }

    void Update()
    {
        if (targets.Count == 0) return; // No targets to consider

        // Find the closest target
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject potentialTarget in targets)
        {
            float distance = Vector3.Distance(potentialTarget.transform.position, this.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = potentialTarget;
            }
        }

        if (closestTarget == null) return;

        if (Vector3.Distance(closestTarget.transform.position, this.transform.position) < 30)
        {
            agent.speed = 15f;
            Evade();
        }
        else
        {
            agent.speed = 3.5f;
            Wonder();
        }
    }
}

