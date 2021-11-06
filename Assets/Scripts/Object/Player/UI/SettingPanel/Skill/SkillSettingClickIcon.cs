using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSettingClickIcon : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public Image borderImage;
    public Image image;
    public Text levelText;
    public UnityAction clickAction;
    private Color _selectBorderColor = new Color(1f, 1f, 0f, 1f);
    private Color _defaultBorderColor = new Color(0.82f, 0.82f, 0.82f, 1f);
    private GameObject _duplicatedObject;
    private bool _isDragStart;
    private DragSkillImage _dragImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction.Invoke();
        SetClickBorder(true);
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

    public void SetClickBorder(bool enabled)
    {
        if(true == enabled)
        {
            borderImage.color = _selectBorderColor;
        }
        else
        {
            borderImage.color = _defaultBorderColor;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (false == _isDragStart)
        {
            _isDragStart = true;
            _duplicatedObject.transform.position = eventData.position;
            _dragImage.SetImage(image.sprite);
            _duplicatedObject.SetActive(true);
        }

        if(_duplicatedObject != null)
        {
            _duplicatedObject.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragStart = false;
        _duplicatedObject.SetActive(false);
    }

    public void SetDuplicateObject(GameObject obj)
    {
        _duplicatedObject = obj;
        _dragImage = _duplicatedObject.GetComponentInChildren<DragSkillImage>();
    }
}
