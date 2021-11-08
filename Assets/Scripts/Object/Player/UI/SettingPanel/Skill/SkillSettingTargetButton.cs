using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSettingTargetButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]private Image borderImage;
    [SerializeField]private Image skillImage;
    private UnityAction _action;

    public void OnPointerClick(PointerEventData eventData)
    {
        _action.Invoke();
    }

    public void SetImage(Sprite sprite)
    {
        skillImage.sprite = sprite;
    }

    public void SetAction(UnityAction action)
    {
        _action = action;
    }
}
