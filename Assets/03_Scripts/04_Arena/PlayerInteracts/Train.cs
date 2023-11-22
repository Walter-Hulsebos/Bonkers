using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers
{
    public class Train : MonoBehaviour
    {
    public Transform[] waypoints;
    public GameObject trainPrefab;
    public float speed = 5f;
    public float breakDuration = 10f;

    private int currentWaypointIndex = 0;

    void Start()
    {
        StartCoroutine(MoveToNextWaypoint());
    }

    IEnumerator MoveToNextWaypoint()
    {
        while (true)
        {
            // Move towards the current waypoint
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // Wait at the waypoint for a break if it's not the first or third waypoint
            if (currentWaypointIndex != 0 && currentWaypointIndex != waypoints.Length - 1)
            {
                yield return new WaitForSeconds(breakDuration);
            }

            // Move to the next waypoint or respawn at the first waypoint
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                // Respawn at the first waypoint
                InstantiateNewTrain();
                Destroy(gameObject);
                yield break;
            }

            currentWaypointIndex++;
        }
    }

    void InstantiateNewTrain()
    {
        Instantiate(trainPrefab, waypoints[0].position, Quaternion.identity);
    }
    }
}
