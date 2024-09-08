using UnityEngine;

public class TankCircularWaypointFollower : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints for the vehicle to follow.
    public float speed = 5.0f;     // Speed of the vehicle.
    public float rotationSpeed = 2.0f; // Speed of rotation when facing the target.

    private Transform currentWaypoint; // The current waypoint to move towards.

    private void Start()
    {
        // If waypoints are assigned, start moving towards a random waypoint.
        if (waypoints != null && waypoints.Length > 0)
        {
            ChooseRandomWaypoint();
        }
    }

    private void Update()
    {
        // If a current waypoint is assigned, move towards it.
        if (currentWaypoint != null)
        {
            // Calculate the direction to the waypoint
            Vector3 direction = currentWaypoint.position - transform.position;
            direction.y = 0f; // Keep the vehicle level with the ground.

            // Rotate towards the waypoint
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move towards the waypoint
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, step);

            // Check if the vehicle has reached the current waypoint.
            if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.1f)
            {
                // Choose a new random waypoint.
                ChooseRandomWaypoint();
            }
        }
    }

    // Function to choose a random waypoint.
    private void ChooseRandomWaypoint()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            int randomIndex = Random.Range(0, waypoints.Length);
            currentWaypoint = waypoints[randomIndex];
        }
        else
        {
            Debug.LogWarning("No waypoints assigned to TankRandomWaypointFollower.");
        }
    }
}