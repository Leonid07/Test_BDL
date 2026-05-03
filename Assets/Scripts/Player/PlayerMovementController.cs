using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles first-person player movement using Unity's new Input System and CharacterController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 verticalVelocity;

    /// <summary>
    /// Initializes required movement components.
    /// </summary>
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Enables movement input actions.
    /// </summary>
    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    /// <summary>
    /// Disables movement input actions.
    /// </summary>
    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

    /// <summary>
    /// Updates player movement and gravity every frame.
    /// </summary>
    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    /// <summary>
    /// Handles horizontal player movement based on input action values.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        bool isSprinting = sprintAction.action.IsPressed();
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Applies gravity to the player controller.
    /// </summary>
    private void HandleGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}