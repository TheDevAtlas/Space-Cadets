using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the player moves
    public Transform cameraTransform;  // Reference to the camera transform
    public Transform planetTransform;  // Reference to the planet's transform

    private Rigidbody rb;
    private Vector2 moveInput;
    private InputSystem_Actions actions;

    private void Start()
    {
        // Initialize the input system
        actions = new InputSystem_Actions();
        actions.Enable();

        // Set up input callbacks for movement
        actions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // Disable default gravity
    }

    private void FixedUpdate()
    {
        // Apply custom spherical gravity towards the planet
        ApplyGravity();

        // Move the player
        MovePlayer();
    }

    private void ApplyGravity()
    {
        // Calculate direction towards the planet's center
        Vector3 gravityDirection = (planetTransform.position - transform.position).normalized;
        Vector3 gravityForce = gravityDirection * 9.81f;  // Replace with desired gravity strength

        // Apply gravity to the Rigidbody
        rb.AddForce(gravityForce, ForceMode.Acceleration);

        // Adjust player orientation to align with the planet's surface
        Vector3 playerUp = -gravityDirection; // Player's up direction is opposite to gravity
        Vector3 playerForward = Vector3.Cross(transform.right, playerUp); // Calculate player's forward direction

        // Set the rotation to align the player's up direction with the calculated up vector
        Quaternion targetRotation = Quaternion.LookRotation(playerForward, playerUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void MovePlayer()
    {
        // Determine the direction to move based on camera orientation relative to the planet's surface
        Vector3 forward = Vector3.Cross(transform.right, -transform.up); // Forward direction relative to the planet
        Vector3 right = Vector3.Cross(-transform.up, transform.forward); // Right direction relative to the planet

        // Calculate the direction to move
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // Calculate the desired movement velocity
        Vector3 desiredVelocity = moveDirection * moveSpeed;

        // Combine the current velocity with the desired movement velocity
        rb.linearVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);
    }
}
