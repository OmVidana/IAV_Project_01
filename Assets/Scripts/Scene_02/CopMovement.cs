using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CopMovement : MonoBehaviour{
    NavMeshAgent agent;
    public List<GameObject> targets = new List<GameObject>(); 
    public float wonderRadius;
    public float wonderDistance;
    public float wonderJitter;

    Vector3 wonderTarget = Vector3.zero;

    void Start(){
        agent = this.GetComponent<NavMeshAgent>();
    }
    void Seek(Vector3 location){
        agent.SetDestination(location);
    }


    void Wonder(){
        wonderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wonderJitter, 0, Random.Range(-1.0f, 1.0f) * wonderJitter);
        wonderTarget.Normalize();
        wonderTarget = wonderTarget * wonderRadius;
        Vector3 targetLocal = wonderTarget + new Vector3(0, 0, wonderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        agent.SetDestination(targetWorld);
    }

    void Update()
    {
        if (targets.Count == 0) return; 

        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject potentialTarget in targets){
            float distance = Vector3.Distance(potentialTarget.transform.position, this.transform.position);
            if (distance < closestDistance){
                closestDistance = distance;
                closestTarget = potentialTarget;
            }
        }

        if (closestTarget == null) return;

        if (Vector3.Distance(closestTarget.transform.position, this.transform.position) < 20 && closestTarget.gameObject.CompareTag("Robber")){
            agent.speed = 15f;
            Seek(closestTarget.transform.position);
        }
        else{
            agent.speed = 6.5f;
            Wonder();
        }
    }
}




