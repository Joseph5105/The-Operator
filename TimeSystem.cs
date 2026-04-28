using UnityEngine;
using TMPro;

public class TimeSystem : MonoBehaviour
{
    public float secondsPerHour = 10f;
    private float timer = 0f;

    public int currentHour = 12;

    public TextMeshProUGUI clockText;

    public CrimeSystem crimeSystem; // 👈 LINKED SYSTEM

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= secondsPerHour)
        {
            timer = 0f;
            AdvanceHour();
        }
    }

    void AdvanceHour()
    {
        currentHour++;
        Debug.Log("Hour: " + currentHour);

        if (currentHour > 12)
            currentHour = 1;

        // 🔥 TRIGGER CRIME SCHEDULE EACH HOUR
        if (crimeSystem != null)
        {
            //The event hook (or notification) for the crime system to schedule a crime each hour
            crimeSystem.ScheduleCrime(secondsPerHour);
        }

        UpdateClock();
    }

    void UpdateClock()
    {
        clockText.text = currentHour + ":00 AM";
    }
}