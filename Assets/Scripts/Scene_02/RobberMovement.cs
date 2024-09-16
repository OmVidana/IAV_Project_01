using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberMovement : MonoBehaviour
{
    NavMeshAgent agent;
    public float evadeDistance = 15.0f;   // Distancia para comenzar a evadir
    public float fleeDistance = 5.0f;     // Distancia para comenzar a huir
    public float seekDistance = 10.0f;    // Distancia para comenzar a perseguir peatones

    public float wanderRadius = 5.0f;     // Radio para deambular
    public float wanderDistance = 10.0f;  // Distancia para deambular
    public float wanderJitter = 1.0f;     // Variaci�n para deambular

    Vector3 wanderTarget = Vector3.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no est� asignado al GameObject: " + gameObject.name);
        }

        // Configuraci�n del NavMeshAgent
        agent.stoppingDistance = 0f;
        agent.autoBraking = false;
        agent.updateRotation = true;
        agent.angularSpeed = 120f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    void Update()
    {
        GameObject targetThreat = DetectThreat();
        GameObject targetPedestrian = DetectPedestrian();

        if (targetThreat != null)
        {
            float distanceToThreat = Vector3.Distance(transform.position, targetThreat.transform.position);

            if (distanceToThreat < fleeDistance)
            {
                // Huir si el polic�a o jugador est� muy cerca
                Flee(targetThreat.transform.position);
            }
            else
            {
                // Evadir al polic�a o jugador
                Evade(targetThreat);
            }
        }
        else if (targetPedestrian != null)
        {
            // Perseguir al peat�n
            Seek(targetPedestrian.transform.position);
        }
        else
        {
            // Comportamiento por defecto: Deambular
            Wander();
        }
    }

    GameObject DetectThreat()
    {
        // Detectar polic�as y jugador (etiquetados como "cop")
        GameObject[] cops = GameObject.FindGameObjectsWithTag("cop");
        GameObject closestThreat = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject cop in cops)
        {
            float distance = Vector3.Distance(transform.position, cop.transform.position);
            if (distance < evadeDistance && distance < closestDistance)
            {
                closestDistance = distance;
                closestThreat = cop;
            }
        }

        return closestThreat;
    }

    GameObject DetectPedestrian()
    {
        // Detectar peatones
        GameObject[] pedestrians = GameObject.FindGameObjectsWithTag("Pedestrian");
        GameObject closestPedestrian = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject pedestrian in pedestrians)
        {
            if (!pedestrian.activeInHierarchy) continue; // Ignorar peatones desactivados

            float distance = Vector3.Distance(transform.position, pedestrian.transform.position);
            if (distance < seekDistance && distance < closestDistance)
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

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = transform.position - location;
        Vector3 newGoal = transform.position + fleeVector.normalized * 10.0f; // Ajusta la distancia seg�n sea necesario
        agent.SetDestination(newGoal);
    }

    void Evade(GameObject target)
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + target.GetComponent<NavMeshAgent>().speed);
        Vector3 futurePosition = target.transform.position + target.transform.forward * lookAhead;
        Flee(futurePosition);
    }

    void Wander()
    {
        wanderTarget += new Vector3(
            Random.Range(-1.0f, 1.0f) * wanderJitter,
            0,
            Random.Range(-1.0f, 1.0f) * wanderJitter
        );

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + Vector3.forward * wanderDistance;
        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        Seek(targetWorld);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedestrian"))
        {
            // Acci�n al atrapar al peat�n
            Debug.Log("Ladr�n ha atrapado al peat�n.");
            other.gameObject.SetActive(false); // Desactivar el peat�n
        }
        else if (other.CompareTag("cop"))
        {
            // Acci�n al ser capturado por polic�a o jugador
            Debug.Log("Ladr�n ha sido capturado por " + other.gameObject.name);
            gameObject.SetActive(false); // Desactivar al ladr�n
            ThievesUI.thievesLeft--;
            ThievesUI.thievesLabel.text = $"Thieves left: {ThievesUI.thievesLeft}";
        }
    }
}
