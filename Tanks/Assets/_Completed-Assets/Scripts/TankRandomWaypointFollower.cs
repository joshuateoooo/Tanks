using UnityEngine;

public class TankRandomWaypointFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float waypointThreshold = 0.1f;

    private Transform currentWaypoint;
    private Transform[] availableWaypoints;

    public void SetRandomWaypoint(Transform[] waypoints)
    {
        availableWaypoints = waypoints;
        ChooseNextWaypoint();
    }

    private void ChooseNextWaypoint()
    {
        if (availableWaypoints.Length > 0)
        {
            int randomIndex = Random.Range(0, availableWaypoints.Length);
            currentWaypoint = availableWaypoints[randomIndex];
        }
        else
        {
            Debug.LogWarning("No waypoints available.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (currentWaypoint != null)
        {
            // Move towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

            // Rotate towards the current waypoint
            Vector3 direction = (currentWaypoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            // Check if we've reached the waypoint
            if (Vector3.Distance(transform.position, currentWaypoint.position) < waypointThreshold)
            {
                ChooseNextWaypoint();
            }
        }
    }
}