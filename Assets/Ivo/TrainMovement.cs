using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public Transform[] waypoints; // An array of three waypoints.
    public float moveSpeed = 2.0f; // Speed of movement between waypoints.
    public float waitTime = 2.0f; // Time to wait at each waypoint.

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0.0f;
    private bool isMovingForward = true;

    private void Start()
    {
        // Initialize the object's position to the first waypoint.
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    private void Update()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned. Please assign waypoints in the Inspector.");
            return;
        }

        if (!isWaiting)
        {
            // Move towards the current waypoint.
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Check if we've reached the current waypoint.
            if (transform.position == targetPosition)
            {
                // Start waiting at the current waypoint.
                isWaiting = true;
                waitTimer = 0.0f;

                // Check if we've reached the last waypoint.
                if (currentWaypointIndex == waypoints.Length - 1 && isMovingForward)
                {
                    isMovingForward = false; // Set the direction to reverse.
                }
                else if (currentWaypointIndex == 0 && !isMovingForward)
                {
                    isMovingForward = true; // Set the direction to forward.
                }

                // Determine the next waypoint based on the direction.
                if (isMovingForward)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    currentWaypointIndex--;
                }
            }
        }
        else
        {
            // Wait at the current waypoint.
            waitTimer += Time.deltaTime;

            // Check if we've waited long enough.
            if (waitTimer >= waitTime)
            {
                // Move to the next waypoint and reset the wait timer.
                isWaiting = false;
            }
        }
    }
}