using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isClick = false;
    private bool _isReady = true;
    private float _timer = 0f;
    private float _cooltime;
    //private ActionInfo _info;
    private ActionType _type;
    [SerializeField] private string _actionName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private SkillCooltimePanel _cooltimePanel;
    public void OnPointerDown(PointerEventData eventData)
    {
        // 쿨타임이나 소모값 등을 계산해야한다 
        if (true == _isReady)
        {
            _isClick = true;
            _isReady = false;
            _timer = _cooltime;
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


    public string GetActionName()
    {
        return _actionName;
    }

    private void Update()
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
}
