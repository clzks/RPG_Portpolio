using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoPanel : MonoBehaviour
{
    [SerializeField]private Text _targetNameText;
    [SerializeField]private Image _targetHpGauge;
    public float panelSustainTime = 5f;
    // SetTargetInfo
    // ActivePanel
    // Timer
    private float timer = 0f;

    public void ActivePanel(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

    public void SetTargetInfo(IActor actor, bool isDead)
    {
        timer = 0f;

        if(true == isDead)
        {
            timer = panelSustainTime - 1f;
        }
        
        _targetNameText.text = actor.GetName();
        _targetHpGauge.fillAmount = actor.GetHpPercent();
        ActivePanel(true);
    }

    private void Update()
    {
        if (true == gameObject.activeSelf)
        {
            timer += Time.deltaTime;

            if (timer >= panelSustainTime)
            {
                ActivePanel(false);
            }
        }
    }
}
