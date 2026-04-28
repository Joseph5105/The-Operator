using UnityEngine;

public class CCTVinteraction : MonoBehaviour, interfaceInteractable
{
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private FirstPersonController playerController;
    [SerializeField]
    private float scrollSpeed = 0.1f, minZoom = 60f, maxZoom = 120f, smoothTime = .01f;

    private Camera mainCamera;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<FirstPersonController>();
        }

        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Debug.Log($"Attempting Zoom {gameObject.name}");
            // Initialize shared zoom values
            CCTVZoomManager.ZoomTarget = mainCamera.fieldOfView;
            CCTVZoomManager.CurrentFOV = mainCamera.fieldOfView;
        }
    }

    public void OnEnter()
    {
        Debug.Log($"Looking at {gameObject.name}");
        if (crosshairUI != null && playerController.IsSeated)
        {
            crosshairUI.SetActive(true);
        }
    }

    void Update()
    {
        if (crosshairUI != null && crosshairUI.activeSelf && mainCamera != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0)
            {
                float newFOV = mainCamera.fieldOfView - scrollInput * scrollSpeed;
                CCTVZoomManager.ZoomTarget = Mathf.Clamp(newFOV, maxZoom, minZoom);
            }

            // Create a local variable to hold the velocity
            float velocity = CCTVZoomManager.ZoomVelocity;

            // Smoothly transition using the shared zoom target
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, CCTVZoomManager.ZoomTarget, ref velocity, smoothTime);

            // Store the velocity back to the static property
            CCTVZoomManager.ZoomVelocity = velocity;
        }
    }

    public void OnExit()
    {
        Debug.Log($"Stopped looking at {gameObject.name}");
        if (crosshairUI != null)
        {
            crosshairUI.SetActive(false);
        }
    }
}