using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class QuestBoard : MonoBehaviour, IPointerClickHandler
{
    public Text title;
    public Text subject;
    private bool _isClear;
    //private QuestManager _questManager;
    private UnityAction _action;

    public void OnPointerClick(PointerEventData eventData)
    {
        _action.Invoke();
    }

    public void SetAction(UnityAction action)
    {
        _action = action;
    }

    public void UpdateText(QuestInfo info, string subjectText, bool isClear)
    {
        if(null == info)
        {
            title.text = "";
            subject.text = "다음 퀘스트 받기";
            return;
        }

        title.text = info.Name;
        subject.text = subjectText;

        _isClear = isClear;
    }
}
