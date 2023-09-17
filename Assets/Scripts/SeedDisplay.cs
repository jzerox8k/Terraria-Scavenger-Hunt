using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedDisplay : MonoBehaviour
{
    public TMP_InputField seedTextValue;

    public void OnCopyButtonClick()
    {
        GUIUtility.systemCopyBuffer = seedTextValue.text;
    }
}
