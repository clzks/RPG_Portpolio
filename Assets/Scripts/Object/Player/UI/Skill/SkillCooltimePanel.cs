using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooltimePanel : MonoBehaviour
{
    public Image cooltimeImage;
    private float _cooltimeRatio;
    
    public void SetCooltime(float ratio)
    {
        _cooltimeRatio = ratio;
    }

    private void Update()
    {
        cooltimeImage.fillAmount = _cooltimeRatio;
    }
}
