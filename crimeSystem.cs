using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CrimeSystem : MonoBehaviour
{
    public List<Crime> possibleCrimes;
    public Crime activeCrime;

    public TextMeshProUGUI crimeLogText;
    public TimeSystem timeSystem;
    public AudioManager audioManager;

    private List<string> crimeLog = new List<string>();
    private int maxLogLines = 10;

    public void ScheduleCrime(float hourDuration)
    {
        float randomDelay = Random.Range(0f, hourDuration);

        Debug.Log("Starting delay for: " + activeCrime.crimeType);
        StartCoroutine(SpawnCrimeAfterDelay(randomDelay));
    }

    IEnumerator SpawnCrimeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnCrime();
    }

    void SpawnCrime()
    {
        // Deactivate previous crime if one exists
        if (activeCrime != null)
        {
            activeCrime.isActive = false;
            HideAllCrimeGameObjects(activeCrime);
        }

        // Set new active crime
        activeCrime = possibleCrimes[Random.Range(0, possibleCrimes.Count)];
        activeCrime.isActive = true;
        ShowAllCrimeGameObjects(activeCrime);

        // Add to crime log
        string timestamp = timeSystem.currentHour.ToString("D2") + ":" + timeSystem.currentMinute.ToString("D2");
        string logEntry = timestamp + " - Call For Service";
        AddCrimeToLog(logEntry);

        // Play crime alert sound
        if (audioManager != null)
        {
            audioManager.PlayCrimeAlert();
        }
    }

    void AddCrimeToLog(string entry)
    {
        crimeLog.Add(entry);

        // Remove oldest entry if exceeds max lines
        if (crimeLog.Count > maxLogLines)
        {
            crimeLog.RemoveAt(0);
        }

        UpdateCrimeLogDisplay();
    }

    void UpdateCrimeLogDisplay()
    {
        crimeLogText.text = string.Join("\n", crimeLog);
    }

    void ShowAllCrimeGameObjects(Crime crime)
    {
        foreach (GameObject obj in crime.crimeGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    void HideAllCrimeGameObjects(Crime crime)
    {
        foreach (GameObject obj in crime.crimeGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}