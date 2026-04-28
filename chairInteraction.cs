using UnityEngine;
using UnityEngine.InputSystem;

public class ChairInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Empty child GameObject positioned at the seat. " +
             "Rotate it to face the desk direction.")]
    public Transform sitPoint;

    [Header("Interaction")]
    [Tooltip("How close the player must be to interact (meters)")]
    public float interactRange = 2.5f;

    [Tooltip("Show a prompt UI when in range (optional)")]
    public GameObject promptUI;

    // Cache
    private FirstPersonController _playerController;  // Changed from PlayerSitController
    private Transform _playerTransform;
    private InputAction _interactAction;

    void Start()
    {
        // Find the player in scene
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<FirstPersonController>();
            _playerTransform = player.transform;
        }

        // Hook into Input System
        var playerInput = _playerTransform?.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            _interactAction = playerInput.actions["Interact"];
            _interactAction.performed += OnInteract;
            _interactAction.Enable();
        }

        if (promptUI) promptUI.SetActive(false);
    }

    void OnDestroy()
    {
        if (_interactAction != null)
            _interactAction.performed -= OnInteract;
    }

    void Update()
    {
        if (_playerTransform == null) return;

        bool inRange = IsPlayerInRange();

        // Show/hide prompt UI
        if (promptUI) promptUI.SetActive(inRange && !_playerController.IsSeated);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_playerController == null) return;

        if (_playerController.IsSeated)
        {
            // Stand up
            _playerController.ForceExitSeatState();
        }
        else if (IsPlayerInRange())
        {
            // Sit down - pass the sitPoint to player controller
            _playerController.SitDown(sitPoint);  // Pass sitPoint here
        }
    }

    private bool IsPlayerInRange()
    {
        if (_playerTransform == null) return false;
        return Vector3.Distance(_playerTransform.position,
                                transform.position) <= interactRange;
    }

}