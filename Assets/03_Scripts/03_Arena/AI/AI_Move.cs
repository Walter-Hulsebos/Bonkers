using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class AI_Move : MonoBehaviour
    {        
    public float roamRadius = 10f;
    public float roamTimer = 5f;
    public float waitTime = 3f; 
    public Vector3 spawnArea = new Vector3(20f, 0f, 20f);

    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    private float timer;
    private bool isWaiting;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = roamTimer;
        
        Vector3 randomSpawnPoint = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        transform.position = randomSpawnPoint;

        PickRandomDestination();
    }

    void Update()
    {
        if (isWaiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isWaiting = false;
                PickRandomDestination();
            }
        }
        else if (agent.remainingDistance < 0.5f)
        {
            isWaiting = true;
            timer = waitTime;
        }
    }

    void PickRandomDestination()
    {
        Vector3 randomPoint = Random.insideUnitSphere * roamRadius;

        Vector3 destination = transform.position + randomPoint;

        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(destination, out navHit, roamRadius, UnityEngine.AI.NavMesh.AllAreas);

        agent.SetDestination(navHit.position);
    }
    }
}
