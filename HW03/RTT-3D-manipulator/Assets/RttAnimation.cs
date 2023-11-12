using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RttAnimation : MonoBehaviour
{
    // DH parameters
    public float l0 = 1f;
    private float q1, q2, q3;

    public Transform endEffector;
    public float deltaTime = 0.05f;
    private float currentTime = 0f;

    // For trajectory drawing
    private LineRenderer lineRenderer;
    private List<Vector3> trajectoryPoints = new List<Vector3>();

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.05f; // Set the width of the trajectory line
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (currentTime <= 3f)
        {
            q1 = Mathf.PI - 4 * currentTime;
            q2 = 3 - 0.5f * currentTime;
            q3 = 2 + currentTime;

            Vector3 endEffectorPosition = EndEffectorPosition();
            endEffector.position = endEffectorPosition;

            // Drawing the trajectory
            trajectoryPoints.Add(endEffectorPosition);
            lineRenderer.positionCount = trajectoryPoints.Count;
            lineRenderer.SetPositions(trajectoryPoints.ToArray());

            currentTime += deltaTime;
        }
    }

    private Vector3 EndEffectorPosition()
    {
        // Compute transformation matrix based on DH parameters

        // Transformation base to joint 1
        Matrix4x4 T01 = DHTransformationMatrix(0, l0, 0, 0);

        // Transformation from joint 1 to joint 2
        Matrix4x4 T12 = DHTransformationMatrix(q1, 0, 0, 0);

        // Transformation from joint2 to joint3
        Matrix4x4 T23 = DHTransformationMatrix(Mathf.PI / 2, q2, 0, Mathf.PI / 2);

        // Transformation from joint3 to end effector
        Matrix4x4 T34 = DHTransformationMatrix(0, q3, 0, 0);

        // Final transformation matrix
        Matrix4x4 FinalT = T01 * T12 * T23 * T34;

        return FinalT.MultiplyPoint(Vector3.zero);
    }

    // Compute the DH transformation matrix
    private Matrix4x4 DHTransformationMatrix(float theta, float d, float a, float alpha)
    {
        Matrix4x4 mat = new Matrix4x4();
        float cosTheta = Mathf.Cos(theta);
        float sinTheta = Mathf.Sin(theta);
        float cosAlpha = Mathf.Cos(alpha);
        float sinAlpha = Mathf.Sin(alpha);

        mat.SetRow(0, new Vector4(cosTheta, -sinTheta * cosAlpha, sinTheta * sinAlpha, a * cosTheta));
        mat.SetRow(1, new Vector4(sinTheta, cosTheta * cosAlpha, -cosTheta * sinAlpha, a * sinTheta));
        mat.SetRow(2, new Vector4(0, sinAlpha, cosAlpha, d));
        mat.SetRow(3, new Vector4(0, 0, 0, 1));

        return mat;
    }
}
