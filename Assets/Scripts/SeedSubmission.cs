using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedSubmission : MonoBehaviour
{
    public TMP_InputField seedTextBox;
    public ItemRandomizerController randomizerController;

    public void OnSeedSubmitButtonClick()
    {
        Debug.Log($"submit: {seedTextBox.text}");

        bool success = randomizerController.ValidateSeed(seedTextBox.text.Trim());

        if (success)
        {
            seedTextBox.text = seedTextBox.text.Trim();
            Debug.Log($"valid seed: {seedTextBox.text}");
        }
        else
        {
            Debug.Log($"invalid seed... : {seedTextBox.text}");
            seedTextBox.text = "Invalid seed...";
        }
    } 
}
