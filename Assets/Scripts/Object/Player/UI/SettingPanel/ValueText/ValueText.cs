using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueText : MonoBehaviour
{
    public Text valueTypeText;
    public Text valueText;

    public void SetText(string valueType, string value)
    {
        valueTypeText.text = valueType;
        valueText.text = value;
    }
}
