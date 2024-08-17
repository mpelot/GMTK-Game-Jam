using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Core core;
    // Start is called before the first frame update
    void Start()
    {
        core = FindFirstObjectByType<Core>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2[] points = calculateTrajectoryPoints(100, 10);

        Vector3[] convertedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            convertedPoints[i] = points[i];
        }
        lineRenderer.positionCount = convertedPoints.Length;
        lineRenderer.SetPositions(convertedPoints);
    }

    private Vector2[] calculateTrajectoryPoints(int pointCount, float timeMultiplier)
    {
        Vector2[] points = new Vector2[pointCount];
        Vector2 position = transform.position;
        Vector2 velocity = ((Vector2) core.transform.position - position).normalized * 0.8f;
        float timeStep = Time.fixedDeltaTime * timeMultiplier;
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = position;

            RaycastHit2D[] hits = Physics2D.RaycastAll(position, velocity, velocity.magnitude * timeStep, LayerMask.GetMask("Planet"));
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider != null) {
                    Vector2 directionToPlanet = (hit.collider.transform.position - (Vector3)position).normalized;
                    float distanceToPlanet = Vector2.Distance(position, hit.collider.transform.position);


                    Vector2 gravitationalForce = directionToPlanet * 0.3f * (2.7f - distanceToPlanet);
                    velocity += gravitationalForce * timeStep;
                }
            }

            position += velocity * timeStep;
        }
        return points;
    }
}
