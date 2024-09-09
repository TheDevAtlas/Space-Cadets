using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;  // Player's transform
    public Transform cameraRotate;  // Transform used for rotating the camera
    public float smoothSpeed = 0.125f;  // Speed for smooth camera follow
    public float mouseSensitivity = 100f;  // Sensitivity for mouse look

    private Vector2 lookInput;
    private InputSystem_Actions actions;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isSprinting = false;
    private bool isFirstSprint = true;  // Track first sprint press

    private void Start()
    {
        actions = new InputSystem_Actions();
        actions.Enable();

        actions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        actions.Player.Sprint.performed += ctx => OnSprintStart();
        actions.Player.Sprint.canceled += ctx => isSprinting = false;

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Handle the camera rotation only when sprinting
        if (isSprinting)
        {
            RotateCamera();
        }

        // Update the camera's orientation to match the player's up direction
        AlignWithPlayerUp();
    }

    private void FixedUpdate()
    {
        // Smoothly follow the target
        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed);
    }

    private void RotateCamera()
    {
        // Calculate the rotation amount based on the look input and sensitivity
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Adjust the x rotation to look up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Clamp to prevent over-rotation

        // Adjust the y rotation to look left and right
        yRotation += mouseX;

        // Apply the rotations to the camera
        cameraRotate.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void AlignWithPlayerUp()
    {
        // Get the player's up direction
        Vector3 playerUp = target.up;

        // Align the camera's up with the player's up direction
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, playerUp) * transform.rotation;

        // Smoothly rotate the camera to align with the player's up direction
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
    }

    private void OnSprintStart()
    {
        // Set sprinting to true and check if it's the first time pressing sprint
        isSprinting = true;

        if (isFirstSprint)
        {
            // Prevent any teleportation by aligning the initial rotation correctly
            xRotation = cameraRotate.localEulerAngles.x;
            yRotation = cameraRotate.localEulerAngles.y;
            isFirstSprint = false;
        }
    }
}
