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
        if (!isMouseOverComputer) return;
        if (isInComputerMode) return;

        isInComputerMode = true;

        // Save seated position FIRST (before moving camera)
        seatedCameraPos = mainCamera.transform.position;
        seatedCameraRot = mainCamera.transform.rotation;

        // Reset CCTV zoom
        if (mainCamera != null)
        {
            CCTVZoomManager.ZoomTarget = 60f;
            CCTVZoomManager.CurrentFOV = 60f;
            CCTVZoomManager.ZoomVelocity = 0f;
            mainCamera.fieldOfView = 60f;
        }

        // NOW snap camera to computer position
        mainCamera.transform.position = computerScreenPosition.position;
        mainCamera.transform.rotation = computerScreenPosition.rotation;

        if (controller != null)
        {
            controller.SetCanMove(false);
            controller.SetCursorLocked(false);
            controller.SetCameraLocked(true);
        }

        computerCanvasGroup.alpha = 1f;

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
        CheckMouseHover();

        if (!isInComputerMode && !returningToSeatView)
            return;

        // 🔴 RETURN TO SEATED VIEW (SNAP)
        if (returningToSeatView)
        {
            if (controller != null)
            {
                controller.SetCanMove(false);
                controller.SetCursorLocked(true);
                controller.SetCameraLocked(false);
            }

            mainCamera.transform.position = seatedCameraPos;
            mainCamera.transform.rotation = seatedCameraRot;
            computerCanvasGroup.alpha = 0f;

            returningToSeatView = false;
        }
    }
} 