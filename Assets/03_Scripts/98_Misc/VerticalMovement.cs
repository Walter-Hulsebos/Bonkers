using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float targetY = 5.0f; // The Y coordinate at which the object will stop.
    public float moveSpeed = 2.0f; // The speed at which the object moves vertically.

    private bool isMovingUp = true; // Indicates if the object is moving up.

    private void Update()
    {
        // Calculate the direction to move based on the current Y position and the target Y.
        Vector3 direction = (targetY > transform.position.y) ? Vector3.up : Vector3.down;

        // Calculate the movement amount based on moveSpeed.
        float movement = moveSpeed * Time.deltaTime;

        // Move the object in the specified direction.
        transform.Translate(direction * movement);

        // Check if the object has reached the target Y coordinate.
        if ((isMovingUp && transform.position.y >= targetY) || (!isMovingUp && transform.position.y <= targetY))
        {
            this.enabled =false;
            // Stop moving when the target Y is reached.
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

            // Toggle the direction for the next movement cycle.
            isMovingUp = !isMovingUp;
        }
    }
}