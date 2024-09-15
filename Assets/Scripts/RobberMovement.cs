using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberMovement : MonoBehaviour
{
    NavMeshAgent agent;
    public List<GameObject> targets = new List<GameObject>(); // List of general targets
    public List<GameObject> pedestrianTargets = new List<GameObject>(); // List of pedestrian targets

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    GameObject GetClosestTarget(List<GameObject> targetList)
    {
        if (targetList.Count == 0)
        {
            return null; // Return null if there are no targets
        }

        GameObject closestTarget = targetList[0];
        float closestDistance = Vector3.Distance(this.transform.position, closestTarget.transform.position);

        foreach (GameObject target in targetList)
        {
            float distance = Vector3.Distance(this.transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    void Pursue()
    {
        GameObject closestTarget = GetClosestTarget(targets);
        if (closestTarget == null) return; // Ensure there's a target to pursue

        Vector3 targetDif = closestTarget.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed + closestTarget.GetComponent<Drive>().currentSpeed);
        if (closestTarget.GetComponent<Drive>().currentSpeed <= 0.01f)
        {
            Seek(closestTarget.transform.position);
            return;
        }
        Seek(closestTarget.transform.position + closestTarget.transform.forward * lookAhead * 5);
    }

    void Evade(GameObject closestTarget)
    {
        Vector3 targetDif = closestTarget.transform.position - this.transform.position;
        float lookAhead = targetDif.magnitude / (agent.speed + 12f/*closestTarget.GetComponent<Drive>().currentSpeed*/);
        if (12f <= 0.01f)
        {
            Flee(closestTarget.transform.position);
            return;
        }
        Flee(closestTarget.transform.position + closestTarget.transform.forward * lookAhead * 5);
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
        return GameObject.FindGameObjectsWithTag("hide");
    }

    void Hide()
    {
        float closestDistance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        GameObject[] hidingSpots = getHidingPlaces();

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hideDirection = hidingSpots[i].transform.position - GetClosestTarget(targets).transform.position;
            Vector3 hidePosition = hidingSpots[i].transform.position + hideDirection.normalized * 5;

            if (Vector3.Distance(this.transform.position, hidePosition) < closestDistance)
            {
                chosenSpot = hidePosition;
                closestDistance = Vector3.Distance(this.transform.position, hidePosition);
            }
        }
        Seek(chosenSpot);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (targets.Contains(collision.gameObject) || pedestrianTargets.Contains(collision.gameObject))
        {
            Time.timeScale = 0f;
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        
        GameObject closestTarget = GetClosestTarget(targets);
        GameObject closestPedestrian = GetClosestTarget(pedestrianTargets);
        if (Vector3.Distance(closestTarget.transform.position, this.transform.position) < 15){
            Evade(closestTarget);
        }else if(Vector3.Distance(closestPedestrian.transform.position, this.transform.position) < 25){
            Seek(closestPedestrian.transform.position);
        }else{

            float closestDistance = Mathf.Infinity;
            Vector3 chosenSpot = Vector3.zero;
            GameObject[] hidingSpots = getHidingPlaces();
            for (int i = 0; i < hidingSpots.Length; i++){
                Vector3 hideDirection = hidingSpots[i].transform.position - closestTarget.transform.position;
                Vector3 hidePosition = hidingSpots[i].transform.position + hideDirection.normalized * 5;

                if (Vector3.Distance(this.transform.position, hidePosition) < closestDistance){
                    chosenSpot = hidePosition;
                    closestDistance = Vector3.Distance(this.transform.position, hidePosition);
                }
            }
            if (closestDistance < 10 ){
                Hide();
            }else{
                Evade(closestPedestrian);
            }
        }
        
        // Combine targets and pedestrian targets to find the closest overall target
        /*List<GameObject> allTargets = new List<GameObject>(targets);
        allTargets.AddRange(pedestrianTargets);

        GameObject closestOverallTarget = GetClosestTarget(allTargets);
        if (Vector3.Distance(closestOverallTarget.transform.position, this.transform.position) < 20){
            Evade();
        }else{
            /*float closestDistance = Mathf.Infinity;
            Vector3 chosenSpot = Vector3.zero;
            GameObject[] hidingSpots = getHidingPlaces();
            for (int i = 0; i < hidingSpots.Length; i++){
                Vector3 hideDirection = hidingSpots[i].transform.position - closestOverallTarget.transform.position;
                Vector3 hidePosition = hidingSpots[i].transform.position + hideDirection.normalized * 5;

                if (Vector3.Distance(this.transform.position, hidePosition) < closestDistance){
                    chosenSpot = hidePosition;
                    closestDistance = Vector3.Distance(this.transform.position, hidePosition);
                }
            }*/
            /*Seek(closestPedestrian.transform.position);
            /*if (Vector3.Distance(closestPedestrian.transform.position, this.transform.position) < 20){
                Seek(closestPedestrian.transform.position);
            }else if (closestDistance < 10 ){
                Hide();
            }else{
                Seek(closestPedestrian.transform.position);
            }*/
        //}
    }
}



