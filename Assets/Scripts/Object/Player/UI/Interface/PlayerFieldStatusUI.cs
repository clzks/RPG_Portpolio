using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerFieldStatusUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image staminaBar;

    public void SetStatusPanel(float hp, float stamina)
    {
        hpBar.fillAmount = hp;
        staminaBar.fillAmount = stamina;
    }
}
