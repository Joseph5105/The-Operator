using UnityEngine;

public class InteractPlayerBox : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    private interfaceInteractable currentInteractable = null;

    private void Update()
    {
        // Ray Cast from the center of the screen
        // Ray cast is like a line from the camera to the world, it can detect if it hits something
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        // Check if the ray hits something within the interact distance and on the interactable layer
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // Check if the hit object has an interactable component
            interfaceInteractable interactable = hit.collider.GetComponent<interfaceInteractable>();

            if(interactable != null && interactable != currentInteractable)
            {
                currentInteractable?.OnExit(); // Call OnExit on the previous interactable if it exists
                currentInteractable = interactable;// Update the current interactable reference
                interactable.OnEnter(); // Call OnEnter on the new interactable
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnExit(); // Call OnExit if we are no longer looking at an interactable
                currentInteractable = null; // Clear the current interactable reference
            }
        }

    }
}
