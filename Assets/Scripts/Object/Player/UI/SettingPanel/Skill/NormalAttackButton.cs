using UnityEngine.EventSystems;

public class NormalAttackButton : ActionButton
{
    public override void Update()
    {
       
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _isClick = true;
    }
}
