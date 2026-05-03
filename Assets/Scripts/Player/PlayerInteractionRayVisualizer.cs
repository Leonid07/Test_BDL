using UnityEngine;

/// <summary>
/// Visualizes the player's interaction raycast using a scaled and oriented cylinder.
/// </summary>
public class PlayerInteractionRayVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInteractionController interactionController;
    [SerializeField] private Transform cylinderTransform;

    [Header("Ray Settings")]
    [SerializeField] private Vector3 rayOffset = new Vector3(0f, -0.1f, 0.1f);
    [SerializeField] private bool showOnlyWhenHit = false;

    [Header("Visual Settings")]
    [SerializeField] private float cylinderRadius = 0.005f;

    /// <summary>
    /// Updates the cylinder visualization every frame.
    /// </summary>
    private void Update()
    {
        if (interactionController == null || cylinderTransform == null)
            return;

        DrawCylinderRay();
    }

    /// <summary>
    /// Positions, rotates, and scales the cylinder to match the raycast.
    /// </summary>
    private void DrawCylinderRay()
    {
        Vector3 startPoint = transform.TransformPoint(rayOffset);
        Vector3 direction = transform.forward;

        float maxDistance = interactionController.InteractionDistance;

        Vector3 endPoint = startPoint + direction * maxDistance;

        bool hasHit = Physics.Raycast(
            startPoint,
            direction,
            out RaycastHit hit,
            maxDistance,
            interactionController.InteractableLayer
        );

        if (hasHit)
            endPoint = hit.point;

        if (showOnlyWhenHit && !hasHit)
        {
            cylinderTransform.gameObject.SetActive(false);
            return;
        }

        cylinderTransform.gameObject.SetActive(true);

        Vector3 middlePoint = (startPoint + endPoint) * 0.5f;
        float length = Vector3.Distance(startPoint, endPoint);

        cylinderTransform.position = middlePoint;
        cylinderTransform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

        cylinderTransform.localScale = new Vector3(
            cylinderRadius,
            length * 0.5f,
            cylinderRadius
        );
    }
}