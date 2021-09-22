using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSettingPanel : MonoBehaviour
{
    public GameObject viewPanel;
    public GameObject screenPanel;

    public void OnClickPanel()
    {
        screenPanel.SetActive(true);
        viewPanel.SetActive(true);
    }

    public void OnClickClosePanel()
    {
        screenPanel.SetActive(false);
        viewPanel.SetActive(false);
    }
}
