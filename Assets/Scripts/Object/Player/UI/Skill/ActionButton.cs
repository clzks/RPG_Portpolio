using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
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
    public bool isDragMode;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (true == _info.IsDragedSkill && true == isDragMode)
            return;

        // 쿨타임이나 소모값 등을 계산해야한다 
        if (true == _isReady)
        {
            _isClick = true;
        }
        else
        {

        }
    }

    // 드래그 모드의 스킬인경우
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (true == _info.IsDragedSkill && true == isDragMode)
        {
            // 드래그 이미지 생성
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (true == _info.IsDragedSkill && true == isDragMode)
        {
            // 드래그 방향에 따라 이미지 회전 및 늘리기
        }
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (true == _info.IsDragedSkill && true == isDragMode)
    //    {
    //
    //    }
    //}

    public void OnPointerUp(PointerEventData eventData)
    {
        if (true == _info.IsDragedSkill && true == isDragMode)
        {

        }
        
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

    public virtual void SetTimer()
    {
        _isReady = false;
        _timer = _cooltime;
        _isClick = false;
    }

    public virtual void SetCooltime(float time)
    {
        _cooltime = time;
    }

    public virtual bool ExecuteButton(float cost)
    {
        if (cost >= _info.Cost)
        {
            SetTimer();

            return true;
        }
        else
        {
            return false;
        }
    }

   
}
