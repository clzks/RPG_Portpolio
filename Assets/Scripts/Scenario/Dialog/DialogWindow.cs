using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour, IPointerClickHandler
{
    public Image portrait;
    public Text narratorNameText;
    public Text narrationText;
    public DialogBlinker blinker;
    public Button skipButton;
    private bool isEndCurrNarration;

    private WaitForSeconds dialogWaitForSecond;
    
    private float dialogSpeed = 0.033f; // 글자 출력 하나당 걸리는 시간

    public DialogInfo currDialogInfo;
    private string currNarator;
    private string currNarration;
    int currIndex = 0;
    private void Awake()
    {
        skipButton.onClick.AddListener(SkipDailog);
        dialogWaitForSecond = new WaitForSeconds(dialogSpeed);
    }
    
    public void SetDialogInfo(DialogInfo info)
    {
        currDialogInfo = info;
        currIndex = 0;
    }

    private void OnEnable()
    {
        UpdateDialog();
    }

    public void UpdateDialog()
    {
        narrationText.text = "";
        currNarator = currDialogInfo.DialogList[currIndex].NarratorName;
        currNarration = currDialogInfo.DialogList[currIndex].Narration;

        narratorNameText.text = currNarator;
        StartCoroutine("NarrationCoroutine");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (false == isEndCurrNarration)
        {
            StopCoroutine("NarrationCoroutine");
            FinishCurrNarration();
        }
        else
        {
            if (currIndex < currDialogInfo.DialogList.Count)
            {
                UpdateDialog();
            }
            else
            {
                EndDialog();
            }
        }
    }

    public void SkipDailog()
    {
        EndDialog();
    }

    public void EndDialog()
    {
        gameObject.SetActive(false);
    }

    public void FinishCurrNarration()
    {
        isEndCurrNarration = true;
        narrationText.text = currNarration;
        currIndex++;
    }

    public IEnumerator NarrationCoroutine()
    {
        isEndCurrNarration = false;

        for(int i = 0; i < currNarration.Length; ++i)
        {
            narrationText.text += currNarration[i];
            yield return dialogWaitForSecond;
        }
        isEndCurrNarration = true;
        currIndex++;
    }
}
