using UnityEngine;

public class ShowAxes : MonoBehaviour
{
    public float axisLength = 1.0f;  // Define the length of the axes

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * axisLength); // X-axis

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * axisLength); // Y-axis

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * axisLength); // Z-axis
    }
}
