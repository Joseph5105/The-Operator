using UnityEngine;
using TMPro;

public class CADSystem : MonoBehaviour
{
    public TMP_Dropdown unitDropdown;
    public TMP_Dropdown crimeDropdown;
    public TMP_Dropdown locationDropdown;
    //TODO: Add a text field for Call ID to track the dispatches calls

    //public TextMeshProUGUI resultText;

    public CrimeSystem crimeManager;

    public void SubmitDispatch()
    {
        string selectedCrime = crimeDropdown.options[crimeDropdown.value].text;
        string selectedLocation = locationDropdown.options[locationDropdown.value].text;

        Crime active = crimeManager.activeCrime;

        if(active != null && active.crimeType == selectedCrime && active.location == selectedLocation)
        {
            //resultText.text = "Dispatch successful! Unit is on the way.";
            Debug.Log("Dispatch successful! Unit is on the way.");
        }
        else
        {
            //resultText.text = "Dispatch failed. No matching crime found.";
            Debug.Log("Dispatch failed. No matching crime found.");
        }
    }

}
