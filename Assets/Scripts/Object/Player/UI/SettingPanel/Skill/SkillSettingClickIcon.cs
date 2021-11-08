using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSettingClickIcon : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    private string _skill;
    public Image borderImage;
    public Image image;
    public Text levelText;
    private UnityAction _clickAction;
    private UnityAction _onDragAction;
    private UnityAction _endDragAction;
    private Color _selectBorderColor = new Color(1f, 1f, 0f, 1f);
    private Color _defaultBorderColor = new Color(0.82f, 0.82f, 0.82f, 1f);
    private GameObject _duplicatedObject;
    private bool _isDragStart;
    private DragSkillImage _dragImage;
    private int _level;

    public void OnPointerClick(PointerEventData eventData)
    {
        _clickAction.Invoke();
        SetClickBorder(true);
    }

    public void SetIcon(Sprite img, int level, string skill)
    {
        _skill = skill;
        image.sprite = img;
        levelText.text = "Lv." + level.ToString();
        _level = level;
    }

    public void SetClickAction(UnityAction action)
    {
        _clickAction = action;
    }

    public void SetOnDragAction(UnityAction action)
    {
        _onDragAction = action;
    }

    public void SetEndDragAction(UnityAction action)
    {
        _endDragAction = action;
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
        if(_level <= 0)
        {
            return;
        }

        if (false == _isDragStart)
        {
            _isDragStart = true;
            _onDragAction.Invoke();
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
        _endDragAction.Invoke();
        _isDragStart = false;
        _duplicatedObject.SetActive(false);
    }

    public void SetDuplicateObject(GameObject obj)
    {
        _duplicatedObject = obj;
        _dragImage = _duplicatedObject.GetComponentInChildren<DragSkillImage>();
    }

    public string GetSkill()
    {
        return _skill;
    }
}
