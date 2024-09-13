using UnityEngine;
using UnityEngine.AI;

public class SteeringBehaviors : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    [Range(0f, 5f)] public float pursueMultiplier = 1.5f;
    [Range(0f, 10f)] public float wanderRadius = 10f;
    [Range(0f, 20f)] public float wanderDistance = 15f;
    [Range(0f, 10f)] public float wanderJitter = 5f;
    private Vector3 _wanderTarget = Vector3.zero;
    public float hideRadius = 2.5f;
    public float evadeRadius = 5f;
    public float fleeRadius = 10f;
    public float wanderRadiusLimit = 16f;
    public Transform[] hidingSpots;
    public bool chaseCop;
    public float chaseRadius = 10f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (chaseCop)
        {
            if (distanceToTarget < chaseRadius)
            {
                Pursue(target.transform.position);
            }
            else
            {
                Wander();
            }
        }
        else
        {
            if (distanceToTarget < hideRadius)
            {
                Hide();
                Debug.Log("Hiding");
            }
            else if (distanceToTarget < evadeRadius)
            {
                Evade();
                Debug.Log("Evading");
            }
            else if (distanceToTarget < fleeRadius)
            {
                Flee();
                Debug.Log("Fleeing");
            }
            else if (distanceToTarget < wanderRadiusLimit)
            {
                Wander();
                Debug.Log("Wanding");
            }
        }
    }

    private void Seek(Vector3 position)
    {
        agent.SetDestination(position);
    }

    private void Flee()
    {
        Vector3 fleeVector = transform.position - target.transform.position;
        agent.SetDestination(transform.position + fleeVector.normalized * fleeRadius);
    }

    private void Pursue(Vector3 position)
    {
        Vector3 fleeVector = position - transform.position;
        float lookAheadFactor = fleeVector.magnitude / (agent.speed + target.GetComponent<Drive>().currentSpeed);
        if (target.GetComponent<Drive>().currentSpeed <= 0.5f)
            Seek(position);
        else
            Seek(target.transform.position + target.transform.forward * (lookAheadFactor * pursueMultiplier));
    }

    private void Evade()
    {
        Vector3 fleeVector = target.transform.position - transform.position;
        float lookAheadFactor = fleeVector.magnitude / (agent.speed + target.GetComponent<Movement>().speed);
        agent.SetDestination(transform.position + fleeVector.normalized * (lookAheadFactor * pursueMultiplier));
    }

    private void Wander()
    {
        _wanderTarget +=
            new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter)
                .normalized;
        _wanderTarget *= wanderRadius;

        Vector3 targetLocal = _wanderTarget + new Vector3(0, 0, wanderDistance);
        // Vector3 targetWorld = transform.InverseTransformVector(targetLocal);
        Vector3 targetWorld = transform.position + targetLocal;

        Seek(targetWorld);
    }

    private void Hide()
    {
        Transform bestHidingSpot = FindClosestHidingSpot();
        if (IsInLineOfSight(bestHidingSpot.position))
            bestHidingSpot = FindNewHidingSpot(bestHidingSpot);

        Seek(bestHidingSpot.position);
    }

    private Transform FindClosestHidingSpot()
    {
        float closestDistance = Mathf.Infinity;
        Transform bestHidingSpot = null;

        foreach (Transform spot in hidingSpots)
        {
            float distanceToSpot = Vector3.Distance(transform.position, spot.position);
            if (distanceToSpot < closestDistance)
            {
                closestDistance = distanceToSpot;
                bestHidingSpot = spot;
            }
        }

        return bestHidingSpot;
    }

    private Transform FindNewHidingSpot(Transform currentSpot)
    {
        foreach (Transform spot in hidingSpots)
        {
            if (spot != currentSpot && !IsInLineOfSight(spot.position))
                return spot;
        }

        return currentSpot;
    }

    private bool IsInLineOfSight(Vector3 position)
    {
        Vector3 directionToTarget = target.transform.position - position;

        if (Physics.Raycast(position, directionToTarget, out var hit))
        {
            if (hit.collider.gameObject == target)
                return true;
        }

        return false;
    }
}