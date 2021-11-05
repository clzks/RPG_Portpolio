using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSettingClickIcon : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Text levelText;
    public UnityAction clickAction;
    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction.Invoke();
    }

    public void SetIcon(Sprite img, int level)
    {
        image.sprite = img;
        levelText.text = "Lv." + level.ToString();
    }

    public void SetClickAction(UnityAction action)
    {
        clickAction = action;
    }
}
