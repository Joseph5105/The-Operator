using UnityEngine;

public class CCTVZoomManager : MonoBehaviour
{
    public static float CurrentFOV { get; set; }
    public static float ZoomTarget { get; set; }
    public static float ZoomVelocity { get; set; }

    private static CCTVZoomManager instance;

    void Awake()//Singleton pattern to ensure only one instance of the manager exists
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}