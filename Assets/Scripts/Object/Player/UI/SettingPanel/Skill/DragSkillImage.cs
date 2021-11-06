using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSkillImage : MonoBehaviour
{
    public Image image;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
