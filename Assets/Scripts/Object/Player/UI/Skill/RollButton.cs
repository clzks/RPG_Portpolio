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
        base.OnPointerDown(eventData);
    }

    public override void Update()
    {
        base.Update();
    }
}
