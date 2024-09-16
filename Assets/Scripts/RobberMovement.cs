using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class RobberMovement : MonoBehaviour
{
    NavMeshAgent agent;
    public List<GameObject> policeOfficers;
    public float fleeDistance = 15.0f;
    public float seekDistance = 10.0f;
    private GameObject currentPedestrianTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no está asignado al GameObject: " + gameObject.name);
        }
    }

    void Update()
    {
        GameObject closestPolice = FindClosestPolice();
        currentPedestrianTarget = FindClosestPedestrian();

        // Evade the police officers if they are nearby
        if (closestPolice != null && Vector3.Distance(this.transform.position, closestPolice.transform.position) < fleeDistance)
        {
            Evade(closestPolice.transform.position);
        }
        // Seek the closest pedestrian if in range
        else if (currentPedestrianTarget != null && Vector3.Distance(this.transform.position, currentPedestrianTarget.transform.position) < seekDistance)
        {
            Seek(currentPedestrianTarget.transform.position);
        }
        else
        {
            Wander();
        }
    }

    GameObject FindClosestPolice()
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestPolice = null;

        foreach (GameObject police in policeOfficers)
        {
            float distance = Vector3.Distance(this.transform.position, police.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPolice = police;
            }
        }

        return closestPolice;
    }

    GameObject FindClosestPedestrian()
    {
        GameObject[] pedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
        float closestDistance = Mathf.Infinity;
        GameObject closestPedestrian = null;

        foreach (GameObject pedestrian in pedestrians)
        {
            float distance = Vector3.Distance(this.transform.position, pedestrian.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPedestrian = pedestrian;
            }
        }

        return closestPedestrian;
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Evade(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Wander()
    {
        float wanderRadius = 10.0f;
        float wanderDistance = 20.0f;
        float wanderJitter = 5.0f;
        Vector3 wanderTarget = new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        agent.SetDestination(targetWorld);
    }

    // Handle collision with pedestrians
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pedestrian"))
        {
            // Stop pursuing the pedestrian
            currentPedestrianTarget = null;

            // Optionally: Destroy pedestrian or trigger another behavior
            Destroy(collision.gameObject); // Destroy pedestrian when caught
            Debug.Log("Pedestrian caught by thief!");
        }
    }
}
