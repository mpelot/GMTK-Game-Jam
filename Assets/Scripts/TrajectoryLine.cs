using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float coreForceMagnitude;

    private Core core;
    // Start is called before the first frame update
    void Start()
    {
        core = FindFirstObjectByType<Core>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2[] points = calculateTrajectoryPoints(1000, 1, 0.2f);

        Vector3[] convertedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            convertedPoints[i] = points[i];
        }
        lineRenderer.positionCount = convertedPoints.Length;
        lineRenderer.SetPositions(convertedPoints);
    }

    private Vector2[] calculateTrajectoryPoints(int pointCount, float timeMultiplier, float radius)
    {
        Vector2[] points = new Vector2[pointCount];
        Vector2 position = transform.position;
        Vector2 velocity = (-position).normalized * 0.8f;
        float timeStep = Time.fixedDeltaTime * timeMultiplier;

        bool endEarly = false;

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = position;

            if (endEarly)
            {
                continue;
            }

            // Calculate gravitational force due to the Core
            Vector2 distanceToCore = (Vector2)core.transform.position - position;
            Vector2 coreForce = distanceToCore.normalized * (coreForceMagnitude / distanceToCore.sqrMagnitude);
            Vector2 totalForce = coreForce;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(position, radius, velocity, velocity.magnitude * timeStep);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Gravity"))
                    {
                        Vector2 directionToPlanet = (hit.collider.transform.position - (Vector3)position).normalized;
                        float distanceToPlanet = Vector2.Distance(position, hit.collider.transform.position);

                        Vector2 gravitationalForce = directionToPlanet * 0.3f * (2.7f - distanceToPlanet);
                        totalForce += gravitationalForce;
                    }
                    else if (hit.collider.CompareTag("Core") || hit.collider.CompareTag("Planet") || hit.collider.CompareTag("Harvester"))
                    {
                        endEarly = true;
                        break;
                    }
                }
            }

            velocity += totalForce * timeStep;

            position += velocity * timeStep;
        }
        return points;
    }
}
