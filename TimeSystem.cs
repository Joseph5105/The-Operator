using UnityEngine;
using TMPro;

public class TimeSystem : MonoBehaviour
{
    public float secondsPerHour = 10f;
    private float timer = 0f;

    public int currentHour = 12;
    public int currentMinute = 0;

    public TextMeshProUGUI clockText;

    public CrimeSystem crimeSystem;

    void Update()
    {
        timer += Time.deltaTime;

        // Update clock display EVERY frame
        UpdateClock();

        if (timer >= secondsPerHour)
        {
            timer = 0f;
            AdvanceHour();
        }
    }

    void AdvanceHour()
    {
        currentHour++;
        currentMinute = 0;
        Debug.Log("Hour: " + currentHour);

        if (currentHour > 12)
            currentHour = 1;

        if (crimeSystem != null)
        {
            crimeSystem.ScheduleCrime(secondsPerHour);
        }
    }

    void UpdateClock()
    {
        // Calculate minutes based on timer progress through the hour
        currentMinute = Mathf.RoundToInt((timer / secondsPerHour) * 60f);

        // Format as HH:MM with leading zeros
        clockText.text = currentHour.ToString("D2") + ":" + currentMinute.ToString("D2") + " AM";
    }
}