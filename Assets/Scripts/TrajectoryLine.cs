using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TrajectoryLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject redx;
    public float coreForceMagnitude;
    public float planetForceMagnitude;
    public Vector2 startingVelocity;
    public float growthLevel = 1f;
    public float circleCastRadius;
    private Collider2D[] startingColliders;
    private Vector2[] trajectoryPoints;

    // Optimizes the FindClosestTrajectoryPoint function by caching the index of the most recent result
    private int mostRecentClosestPointIndex = 0;

    private Core core;
    // Start is called before the first frame update
    void Start()
    {
        core = FindFirstObjectByType<Core>();
        startingColliders = Physics2D.OverlapCircleAll(transform.position, circleCastRadius);
        redx.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        trajectoryPoints = CalculateTrajectoryPoints(1000, 10 * growthLevel, circleCastRadius);

        Vector3[] convertedPoints = new Vector3[trajectoryPoints.Length];
        for (int i = 0; i < trajectoryPoints.Length; i++)
        {
            convertedPoints[i] = trajectoryPoints[i];
        }
        lineRenderer.positionCount = convertedPoints.Length;
        lineRenderer.SetPositions(convertedPoints);
    }

    public void Show()
    {
        lineRenderer.enabled = true;
    }    
    
    public void Hide()
    {
        lineRenderer.enabled = false;
    }

    private Vector2[] CalculateTrajectoryPoints(int pointCount, float timeMultiplier, float radius)
    {
        Vector2[] points = new Vector2[pointCount];
        Vector2 position = transform.position;
        Vector2 velocity = startingVelocity;
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
            Vector2 coreForce = (distanceToCore.normalized * (coreForceMagnitude / distanceToCore.sqrMagnitude) / (growthLevel * growthLevel));
            if (Mathf.Abs(coreForce.x) == Mathf.Infinity || Mathf.Abs(coreForce.y) == Mathf.Infinity)
            {
                coreForce = Vector2.zero;
            }
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

                        Vector2 gravitationalForce = (directionToPlanet * planetForceMagnitude * (2.7f - distanceToPlanet)) / (growthLevel * growthLevel);
                        if (Mathf.Abs(gravitationalForce.x) == Mathf.Infinity || Mathf.Abs(gravitationalForce.y) == Mathf.Infinity)
                        {
                            gravitationalForce = Vector2.zero;
                        }
                        if (growthLevel > 5)
                        {
                            gravitationalForce = Vector2.zero;
                        }
                        totalForce += gravitationalForce;
                    }
                    else if (hit.collider.CompareTag("Core") || hit.collider.CompareTag("Planet"))
                    {
                        if (!IsInStartingColliders(hit.collider))
                        {
                            endEarly = true;
                            break;
                        }

                    }
                    else if (hit.collider.CompareTag("Harvester"))
                    {
                        if (growthLevel > 5)
                        {
                            redx.SetActive(true);
                            redx.transform.position = hit.point;
                        }
                        else
                        {
                            redx.SetActive(false);
                        }

                        if (!IsInStartingColliders(hit.collider))
                        {
                            endEarly = true;
                            break;
                        }
                    }
                    else
                    {
                        redx.SetActive(false);
                    }
                }
            }

            velocity += totalForce * timeStep;

            position += velocity * timeStep;
        }
        return points;
    }


    private bool IsInStartingColliders(Collider2D collider)
    {
        foreach (Collider2D startingCollider in startingColliders)
        {
            if (collider == startingCollider)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2 FindClosestTrajectoryPoint(Vector2 position)
    {
        bool isSearchingUp;
        int i = mostRecentClosestPointIndex;

        if (mostRecentClosestPointIndex <= 0)
        {
            isSearchingUp = true;
            i = 0;
        }
        else if (mostRecentClosestPointIndex >= trajectoryPoints.Length - 1)
        {
            isSearchingUp = false;
            i = trajectoryPoints.Length - 1;
        }
        else
        {
            float distanceDown = (trajectoryPoints[i - 1] - position).magnitude;
            float distanceUp = (trajectoryPoints[i + 1] - position).magnitude;
            isSearchingUp = distanceUp < distanceDown;
        }

        float smallestDistance = (trajectoryPoints[i] - position).magnitude;

        if (isSearchingUp)
        {
            while (i < trajectoryPoints.Length - 1)
            {
                float distance = (trajectoryPoints[i + 1] - position).magnitude;
                if (distance > smallestDistance)
                {
                    mostRecentClosestPointIndex = i;
                    return trajectoryPoints[i];
                }
                smallestDistance = distance;
                i++;
            }
        }
        else
        {
            while (i > 1)
            {
                float distance = (trajectoryPoints[i - 1] - position).magnitude;
                if (distance > smallestDistance)
                {
                    mostRecentClosestPointIndex = i;
                    return trajectoryPoints[i];
                }
                smallestDistance = distance;
                i--;
            }
        }

        mostRecentClosestPointIndex = i;
        return trajectoryPoints[i];
    }
}
