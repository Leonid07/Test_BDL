using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles first-person camera rotation using Unity's new Input System.
/// </summary>
public class PlayerLookController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference lookAction;

    [Header("Look Settings")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;

    private float xRotation;

    /// <summary>
    /// Locks and hides the cursor when the controller starts.
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Enables look input action.
    /// </summary>
    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    /// <summary>
    /// Disables look input action.
    /// </summary>
    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    /// <summary>
    /// Updates camera rotation every frame.
    /// </summary>
    private void LateUpdate()
    {
        HandleLook();
    }

    /// <summary>
    /// Rotates the player camera and body based on look input.
    /// </summary>
    private void HandleLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}