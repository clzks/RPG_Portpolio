using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    public Toggle toggle;
    public GameObject buttonGroup;
    public Vector3 originPosX;
    public Vector3 destPosX;
    private float _toggleTimer;
    private Vector3 Position { get { return buttonGroup.transform.position; }set { buttonGroup.transform.position = value; } }
    private void Awake()
    {
        originPosX = transform.position;
        destPosX = originPosX - new Vector3(-1000, 0, 0);
        toggle.onValueChanged.AddListener(delegate { ResetToggleTimer(toggle); });
    }

    private void Update()
    {
        _toggleTimer += Time.deltaTime * 0.5f;
        if(_toggleTimer >= 1f)
        {
            _toggleTimer = 1f;
        }

        if (true == toggle.isOn)
        {
            Position = Vector3.Lerp(Position, destPosX, _toggleTimer);
        }
        else
        {
            Position = Vector3.Lerp(Position, originPosX, _toggleTimer);
        }
    }

    public void ResetToggleTimer(Toggle toggle)
    {
        _toggleTimer = 0f;
    }
}
