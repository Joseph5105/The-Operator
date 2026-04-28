using UnityEngine;

public class CADInteraction : MonoBehaviour, IComputerInteractable
{
    [SerializeField] private Transform computerScreenPosition;
    [SerializeField] private Camera mainCamera;
    //[SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private CanvasGroup computerCanvasGroup;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private FirstPersonController playerController;

    [SerializeField] private GameObject interactPromptCanvasGroup;

    private bool isInComputerMode = false;
    public bool IsInComputerMode => isInComputerMode;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    // Stores seated view separately
    private Vector3 seatedCameraPos;
    private Quaternion seatedCameraRot;

    private float transitionTimer = 0f;
    private bool returningToSeatView = false;

    private FirstPersonController controller;

    private bool isMouseOverComputer = false;

    void Start()
    {

        if (mainCamera == null)
            mainCamera = Camera.main;

        controller = FindFirstObjectByType<FirstPersonController>();

        originalCameraPos = mainCamera.transform.position;
        originalCameraRot = mainCamera.transform.rotation;

        computerCanvasGroup.alpha = 1f;

        if(interactPromptCanvasGroup != null)
        {
            interactPromptCanvasGroup.SetActive(false); // Start with prompt hidden
        }
    }

    public void EnterComputerMode()
    {
        if (!isMouseOverComputer) return; // Only allow entering computer mode if mouse is over the computer
        if (isInComputerMode) return;

        isInComputerMode = true;
        //transitionTimer = 0f;

        if(mainCamera != null)
        {
            CCTVZoomManager.ZoomTarget = 60f; // Reset zoom when exiting computer mode
            CCTVZoomManager.CurrentFOV = 60f; // Ensure FOV is reset immediately
            CCTVZoomManager.ZoomVelocity = 0f;// Reset zoom velocity to prevent carryover

            mainCamera.fieldOfView = CCTVZoomManager.CurrentFOV; // Ensure FOV is reset immediately
        }


        // Captures current seated camera state
        seatedCameraPos = mainCamera.transform.position;
        seatedCameraRot = mainCamera.transform.rotation;

        if (controller != null)
        {
            controller.SetCanMove(false);
            controller.SetCursorLocked(false);
        }

        if (playerAnimator != null)
            playerAnimator.SetBool("IsSitting", true);
    }

    public void ExitComputerMode()
    {
        if (!isInComputerMode) return;

        isInComputerMode = false;
        //transitionTimer = 0f;

        returningToSeatView = true;

        if (controller != null)
        {
            controller.SetCanMove(false);
            controller.SetCursorLocked(true);
        }

        if (playerAnimator != null)
            playerAnimator.SetBool("IsSitting", false);
    }

    public void RequestExit()
    {
        ExitComputerMode();
    }

    private void CheckMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if raycast hits THIS object's collider
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            isMouseOverComputer = true;
        }
        else
        {
            isMouseOverComputer = false;
        }

        // 🔵 Update prompt visibility only when player is not already in computer mode and is seated (to prevent prompt from showing when they are already at the computer)
        if (isMouseOverComputer && !isInComputerMode && playerController.IsSeated)
        {
            interactPromptCanvasGroup.SetActive(true);
        }
        else
        {
            interactPromptCanvasGroup.SetActive(false);
        }
    }

    void Update()
    {

        CheckMouseHover();// Initial check to set mouse hover state

        if (!isInComputerMode && transitionTimer == 0f && !returningToSeatView)
            return;

        // Note: The original code had smooth transitions using Lerp, but for a more immediate snap to position, we can directly set the camera's position and rotation without interpolation. If you want to keep the smooth transition, you can uncomment the Lerp code and adjust the transitionDuration as needed.
        // 🔵 ENTER COMPUTER MODE
        /*if (isInComputerMode)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            mainCamera.transform.position =
                Vector3.Lerp(seatedCameraPos, computerScreenPosition.position, t);

            mainCamera.transform.rotation =
                Quaternion.Lerp(seatedCameraRot, computerScreenPosition.rotation, t);

            computerCanvasGroup.alpha = t;
        }
        // 🔴 RETURN TO SEATED VIEW
        else if (returningToSeatView)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            mainCamera.transform.position =
                Vector3.Lerp(computerScreenPosition.position, seatedCameraPos, t);

            mainCamera.transform.rotation =
                Quaternion.Lerp(computerScreenPosition.rotation, seatedCameraRot, t);

            computerCanvasGroup.alpha = 1f - t;

            if (t >= 1f)
            {
                returningToSeatView = false;
                transitionTimer = 0f;
            }
        }*/
        // 🔵 ENTER COMPUTER MODE (SNAP)
        if (isInComputerMode)
        {
            if (controller != null)
            {
                controller.SetCanMove(false);
                controller.SetCursorLocked(false); // cursor FREE for UI
                controller.SetCameraLocked(true);  // camera frozen
            }
            mainCamera.transform.position = computerScreenPosition.position;
            mainCamera.transform.rotation = computerScreenPosition.rotation;

            computerCanvasGroup.alpha = 1f;
        }

        // 🔴 RETURN TO SEATED VIEW (SNAP)
        else if (returningToSeatView)
        {
            if (controller != null)
            {
                controller.SetCanMove(false);
                controller.SetCursorLocked(true);  // lock cursor back to center
                controller.SetCameraLocked(false); // restore camera control
            }
            mainCamera.transform.position = seatedCameraPos;
            mainCamera.transform.rotation = seatedCameraRot;

            computerCanvasGroup.alpha = 1f;

            returningToSeatView = false;
            transitionTimer = 0f;
        }
    }
} 