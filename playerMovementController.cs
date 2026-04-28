using UnityEngine;
using System.Collections;

//Coroutine is a way to run code over multiple frames without blocking the main thread. It's useful for things like smooth transitions, waiting for events, or creating timed effects.
//Quaterinion is a way to represent rotations in 3D space. It avoids issues like gimbal lock that can occur with Euler angles. Quaternions consist of four components (x, y, z, w) and can be used to smoothly interpolate between rotations.

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 10f;

    [Header("Gravity")]
    public float gravity = -15f;

    [Header("Mouse Look")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float lookXLimit = 80f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.1f;

    [Header("Sit Settings")]
    public float sitTransitionSpeed = 5f;
    public float seatedLookXLimit = 60f;

    private CharacterController controller;
    private Vector3 moveVelocity;
    private Vector3 verticalVelocity;

    private float rotationX = 0f;
    private bool isGrounded;
    private bool canMove = true;

    private bool isSeated = false;
    private Transform sitPoint;
    private Vector3 standPosition;
    private Coroutine sitTransitionCoroutine;

    public bool IsSeated => isSeated;
    private bool cameraLocked = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (playerCamera == null)
            playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();

        HandleGlobalInputs();

        if (canMove && !isSeated && controller.enabled)
        {
            HandleMovement();
        }
    }

    void HandleGlobalInputs()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CADInteraction computer = FindFirstObjectByType<CADInteraction>();

            bool inCAD = computer != null && computer.IsInComputerMode;

            // 🔵 If in CAD → only exit CAD, DO NOT stand up
            if (inCAD)
            {
                computer.RequestExit();
                return;
            }

            // 🔴 Only stand up if NOT in CAD
            if (isSeated)
            {
                ForceExitSeatState();
            }

            return;
        }

        // ✅ E = interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteract();
        }
    }

    void HandleMovement()
    {
        if (!controller.enabled)
            return;

        CheckGround();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 input = (transform.right * moveX + transform.forward * moveZ).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 targetVelocity = input * speed;
        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, acceleration * Time.deltaTime);

        if (isGrounded)
            verticalVelocity.y = -2f;
        else
            verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 finalMotion = (moveVelocity + verticalVelocity) * Time.deltaTime;
        controller.Move(finalMotion);
    }

    // ---------------- INTERACT ----------------
    void HandleInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (isSeated)
        {
            CADInteraction computer = FindFirstObjectByType<CADInteraction>();

            if (computer != null &&
                Vector3.Distance(transform.position, computer.transform.position) <= 5f &&
                !computer.IsInComputerMode)
            {
                computer.EnterComputerMode();
            }

            return;
        }

        ChairInteraction closestChair = null;
        float chairDist = float.MaxValue;

        ChairInteraction[] chairs = FindObjectsByType<ChairInteraction>(FindObjectsSortMode.None);
        foreach (ChairInteraction chair in chairs)
        {
            float dist = Vector3.Distance(transform.position, chair.transform.position);
            if (dist < chairDist && dist <= chair.interactRange)
            {
                chairDist = dist;
                closestChair = chair;
            }
        }

        if (closestChair != null)
        {
            SitDown(closestChair.sitPoint);
            SitDown(closestChair.sitPoint);
        }
    }

    // ---------------- SIT ----------------
    public void SitDown(Transform seatPoint)
    {
        if (isSeated) return;

        sitPoint = seatPoint;
        standPosition = transform.position;

        moveVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;

        isSeated = true;
        canMove = false;

        if (sitTransitionCoroutine != null)
            StopCoroutine(sitTransitionCoroutine);

        sitTransitionCoroutine = StartCoroutine(TransitionToSit());
    }

    // ---------------- STAND ----------------
    /*public void StandUp()
    {
        if (!isSeated) return;

        isSeated = false;
        canMove = true;
        sitPoint = null;

        rotationX = 0f;
        playerCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (sitTransitionCoroutine != null)
            StopCoroutine(sitTransitionCoroutine);

        sitTransitionCoroutine = StartCoroutine(TransitionToStand());
    }*/

    IEnumerator TransitionToSit()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, sitPoint.eulerAngles.y, 0f);

        float elapsed = 0f;
        float duration = 1f;

        controller.enabled = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            transform.position = Vector3.Lerp(startPos, sitPoint.position, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = sitPoint.position;
        transform.rotation = targetRot;
        controller.enabled = true;
    }

    public void ForceExitSeatState()
    {

        if (!isSeated) return;

        if (playerCamera != null)
        {
            CCTVZoomManager.ZoomTarget = 60f; // Reset zoom when exiting computer mode
            CCTVZoomManager.CurrentFOV = 60f; // Ensure FOV is reset immediately
            CCTVZoomManager.ZoomVelocity = 0f;// Reset zoom velocity to prevent carryover

            playerCamera.fieldOfView = CCTVZoomManager.CurrentFOV; // Ensure FOV is reset immediately
        }

        isSeated = false;
        canMove = true;
        sitPoint = null;

        rotationX = 0f;
        playerCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (sitTransitionCoroutine != null)
            StopCoroutine(sitTransitionCoroutine);

        sitTransitionCoroutine = StartCoroutine(TransitionToStand());
    }

    IEnumerator TransitionToStand()
    {
        Vector3 startPos = transform.position;

        float elapsed = 0f;
        float duration = 1f / sitTransitionSpeed;

        controller.enabled = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            transform.position = Vector3.Lerp(startPos, standPosition, t);
            yield return null;
        }

        transform.position = standPosition;
        controller.enabled = true;
    }

    void CheckGround()
    {
        Vector3 bottom = transform.position + controller.center - Vector3.up * (controller.height / 2f);
        isGrounded = Physics.CheckSphere(bottom, groundCheckDistance, groundMask);
    }

    public void SetCameraLocked(bool locked)
    {
        cameraLocked = locked;
    }

    void HandleMouseLook()
    {

        if (cameraLocked)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            moveVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
        }
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    // ---------------- ANIMATION SUPPORT ----------------

    // Returns horizontal movement speed (ignores vertical velocity)
    public float GetVelocityMagnitude()
    {
        return new Vector3(moveVelocity.x, 0f, moveVelocity.z).magnitude;
    }

    // Returns true if player is moving
    public bool IsMoving()
    {
        return GetVelocityMagnitude() > 0.1f;
    }

    // Returns grounded state
    public bool IsGrounded()
    {
        return isGrounded;
    }
}