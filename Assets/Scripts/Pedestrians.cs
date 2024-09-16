using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Pedestrians : MonoBehaviour
{
    NavMeshAgent agent;
    public List<GameObject> targets = new List<GameObject>(); 
    public float wonderRadius;
    public float wonderDistance;
    public float wonderJitter;

    Vector3 wonderTarget = Vector3.zero;

    void Start(){
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Flee(Vector3 location){
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Evade()
    {
        if (targets.Count == 0) return; 

        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;
        foreach (GameObject potentialTarget in targets){
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
        else
        {
            Flee(closestTarget.transform.position + closestTarget.transform.forward * lookAhead * 5);
        }
    }

    void Wonder()
    {
        
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

        if (Vector3.Distance(closestTarget.transform.position, this.transform.position) < 10)
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

