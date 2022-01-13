using UnityEngine.EventSystems;

public class NormalAttackButton : ActionButton
{
    private string _actionName = "Attack0";

    public override void Update()
    {
       
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _isClick = true;
    }

    public override string GetActionName()
    {
        return _actionName;
    }

    public override void ExecuteButton()
    {
        _isClick = false;
    }
}
