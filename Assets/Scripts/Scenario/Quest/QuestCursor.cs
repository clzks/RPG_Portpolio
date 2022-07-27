using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCursor : MonoBehaviour
{
    public Image cursor;
    public Text distanceText;

    public void SetActive(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

    public void SetCursor(Vector3 angle, int distance)
    {
        cursor.rectTransform.eulerAngles = angle;
        distanceText.text = distance.ToString() + "m";
    }
}
