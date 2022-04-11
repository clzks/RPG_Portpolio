using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerFieldStatusUI : MonoBehaviour
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Image _expBar;
    [SerializeField] private Transform _buffIconContentParent;

    public void AddBuffIcon(BuffIcon icon)
    {
        icon.transform.SetParent(_buffIconContentParent);
    }

    public void UpdateStatusPanel(float hp, float stamina, float exp = 0f)
    {
        _hpBar.fillAmount = hp;
        _staminaBar.fillAmount = stamina;
        _expBar.fillAmount = exp;
    }
}
