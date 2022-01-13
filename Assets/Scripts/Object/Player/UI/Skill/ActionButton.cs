using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int buttonListIndex;
    protected bool _isClick = false;
    private bool _isReady = true;
    private float _timer = 0f;
    private float _cooltime;
    private ActionInfo _info;
    [SerializeField]protected ActionType _type;
    //[SerializeField] private string _actionName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private SkillCooltimePanel _cooltimePanel;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // 쿨타임이나 소모값 등을 계산해야한다 
        if (true == _isReady)
        {
            _isClick = true;
        }
        else
        {

        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _isClick = false;
    }

    public bool GetButtonDown()
    {
        return _isClick;
    }


    public virtual string GetActionName()
    {
        if (null == _info)
        {
            return string.Empty;
        }
        else
        {
            return _info.Name;
        }
    }

    public virtual void Update()
    {
        if (false == _isReady)
        {
            _timer -= Time.deltaTime;
            _cooltimePanel.SetCooltime(_timer / _cooltime);

            if(_timer <= 0f)
            {
                _isReady = true;
            }
        }
    }

    public void SetAction(ActionInfo info, Sprite image)
    {
        _info = info;
        if (info.Type == ActionType.Skill)
        {
            _skillImage.sprite = image;
        }
        _cooltime = _info.CoolTime;
    }

    public void ResetAction()
    {
        _info = null;
        _skillImage.sprite = null;
        _isReady = true;
        _cooltime = 0f;
        _cooltimePanel.SetCooltime(0);
    }

    public virtual void ExecuteButton()
    {
        _isReady = false;
        _timer = _cooltime;
        _isClick = false;
    }
}
