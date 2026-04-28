using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FirstPersonController))]
public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    private FirstPersonController fpsController;

    void Start()
    {
        fpsController = GetComponent<FirstPersonController>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Use the horizontal + forward velocity magnitude
        float speed = fpsController.GetVelocityMagnitude();
        animator.SetFloat("Speed", speed);
    }
}