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

    public override void OnPointerUp(PointerEventData eventData)
    {
        _isClick = false;
    }

    public override string GetActionName()
    {
        return _actionName;
    }

    public override bool ExecuteButton(float currStamina = 0f)
    {
        return _isClick;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // ���ݹ�ư�� �巡�� ���ǹ�
    }

    public override void OnDrag(PointerEventData eventData)
    {
        // ���ݹ�ư�� �巡�� ���ǹ�
    }
}
