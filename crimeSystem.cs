using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimeSystem : MonoBehaviour
{
    public List<Crime> possibleCrimes;

    public Crime activeCrime;

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
        activeCrime = possibleCrimes[Random.Range(0, possibleCrimes.Count)];

        Debug.Log("🚨 Crime Spawned: " + activeCrime.crimeType);
        Debug.Log("Location: " + activeCrime.location);
        Debug.Log("Time slot event");
    }
}