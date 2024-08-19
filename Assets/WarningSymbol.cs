using UnityEngine;

public class WarningSymbol : MonoBehaviour
{
    private Transform parentObject;
    private Camera mainCamera;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        parentObject = transform.parent;
        mainCamera = Camera.main;
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (parentObject == null || mainCamera == null || circleCollider == null) return;

        // Convert parent's position to screen space
        Vector3 parentScreenPosition = mainCamera.WorldToScreenPoint(parentObject.position);

        // Calculate the collider radius in screen space
        float radiusInWorldUnits = circleCollider.radius * transform.localScale.x; // assuming uniform scaling
        float radiusInScreenUnits = radiusInWorldUnits * (Screen.height / (2f * mainCamera.orthographicSize));

        // Check if the parent is on screen
        bool isOnScreen = parentScreenPosition.x >= radiusInScreenUnits && parentScreenPosition.x <= Screen.width - radiusInScreenUnits &&
                          parentScreenPosition.y >= radiusInScreenUnits && parentScreenPosition.y <= Screen.height - radiusInScreenUnits;

        if (isOnScreen)
        {
            // If the parent is on screen, follow the parent
            transform.position = parentObject.position;
        }
        else
        {
            // If the parent is off screen, find the intersection point
            Vector3 originScreenPosition = mainCamera.WorldToScreenPoint(Vector3.zero);
            Vector3 direction = (parentScreenPosition - originScreenPosition).normalized;

            // Find the intersection point with the screen edges
            Vector3 intersection = FindIntersectionWithScreenEdges(originScreenPosition, direction, radiusInScreenUnits);

            // Convert the intersection point back to world space
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(intersection);
            worldPosition.z = transform.position.z; // Keep the same z position

            // Update the position of the StayOnScreen object
            transform.position = worldPosition;
        }
    }

    private Vector3 FindIntersectionWithScreenEdges(Vector3 origin, Vector3 direction, float radiusInScreenUnits)
    {
        float tMaxX = Mathf.Infinity;
        float tMaxY = Mathf.Infinity;

        if (direction.x != 0)
        {
            if (direction.x > 0)
                tMaxX = (Screen.width - radiusInScreenUnits - origin.x) / direction.x;
            else
                tMaxX = (radiusInScreenUnits - origin.x) / direction.x;
        }

        if (direction.y != 0)
        {
            if (direction.y > 0)
                tMaxY = (Screen.height - radiusInScreenUnits - origin.y) / direction.y;
            else
                tMaxY = (radiusInScreenUnits - origin.y) / direction.y;
        }

        float tMax = Mathf.Min(tMaxX, tMaxY);
        return origin + tMax * direction;
    }
}
