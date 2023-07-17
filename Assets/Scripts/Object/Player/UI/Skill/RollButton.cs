using UnityEngine;
using UnityEngine.EventSystems;

public class RollButton : ActionButton
{
    private string _actionName = "Roll";

    private void Awake()
    {
        SetCooltime(3f);
    }

    public override string GetActionName()
    {
        return _actionName;
    }

    public override bool ExecuteButton(float cost)
    {
        var isClick = _isClick;

        if(true == isClick)
        {
            SetTimer();
        }

        return isClick;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _isClick = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _isClick = false;
    }

    public override void Update()
    {
        base.Update();
    }
}
