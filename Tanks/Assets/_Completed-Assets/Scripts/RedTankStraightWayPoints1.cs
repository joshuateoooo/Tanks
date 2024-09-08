using UnityEngine;

public class RedTankStraightWayPointFollower : MonoBehaviour
{
    public Transform[] waypoints;   // Array of waypoints for the vehicle to follow.
    public float speed = 5.0f;      // Speed of the vehicle.

    private int currentWaypointIndex; // Index of the current waypoint.

    private void Start()
    {
        // If waypoints are assigned, start moving towards the first waypoint.
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypointIndex = 0;
            SetDestination(waypoints[currentWaypointIndex]);
        }
    }

    private void Update()
    {
        // If waypoints are assigned, move towards the current waypoint.
        if (waypoints != null && waypoints.Length > 0)
        {
            float step = speed * Time.deltaTime;

            // Move the vehicle towards the current waypoint.
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, step);

            // Check if the vehicle has reached the current waypoint.
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                // Move to the next waypoint.
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                SetDestination(waypoints[currentWaypointIndex]);
            }
        }
    }

    // Function to set the destination (waypoint) for the vehicle.
    private void SetDestination(Transform destination)
    {
        // Rotate the vehicle to face the destination.
        Vector3 direction = destination.position - transform.position;
        direction.y = 0f; // Keep the vehicle level with the ground.
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = toRotation;
    }
}
