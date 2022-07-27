using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private Vector3 _cameraDistance = new Vector3(0, 30f, -20.1f);
    private Vector3 _originEuler = new Vector3(55, 0, 0);
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    private float _limitR = 90f;
    private float _limitL = 9f;
    private float _limitU = 70f;
    private float _limitB = -6f;
    [SerializeField]private GameObject _uiCanvas;
    [SerializeField] private GameObject _movePadPanel;
    [SerializeField] private GameObject _actionPadPanel;
    [SerializeField] private GameObject _fieldStatusPanel;
    [SerializeField] private GameObject _questBoardPanel;
    [SerializeField] private GameObject _optionPanel;
    [SerializeField] private GameObject _tutorialPanel;

    [SerializeField]private GameObject _miniMap;
    [SerializeField] private SpriteRenderer _cutBorder;

    public void SetCameraDistance(Vector3 playerPos)
    {
        //_cameraDistance = transform.position - playerPos;
    }

    public void FollowPlayer(Vector3 playerPos)
    {
        transform.position = playerPos + _cameraDistance;

        if(Position.x <= _limitL)
        {
            Position = new Vector3(_limitL, Position.y, Position.z);
        }
        else if(Position.x >= _limitR)
        {
            Position = new Vector3(_limitR, Position.y, Position.z);
        }

        if (Position.z <= _limitB)
        {
            Position = new Vector3(Position.x, Position.y, _limitB);
        }
        else if(Position.z >= _limitU)
        {
            Position = new Vector3(Position.x, Position.y, _limitU);
        }
    }

    public void SetActiveUI(bool enabled)
    {
        _miniMap.SetActive(enabled);
        _uiCanvas.SetActive(enabled);
    }

    public void SetActiveMovePadPanel(bool enabled)
    {
        if (enabled != _movePadPanel.activeSelf)
        {
            _movePadPanel.SetActive(enabled);
        }
    }

    public void SetActiveActionPadPanel(bool enabled)
    {
        if (enabled != _actionPadPanel.activeSelf)
        {
            _actionPadPanel.SetActive(enabled);
        }
    }

    public void SetActiveStatusPanel(bool enabled)
    {
        if (enabled != _fieldStatusPanel.activeSelf)
        {
            _fieldStatusPanel.SetActive(enabled);
        }
    }
    public void SetActiveQuestPanel(bool enabled)
    {
        if (enabled != _questBoardPanel.activeSelf)
        {
            _questBoardPanel.SetActive(enabled);
        }
    }

    public void SetActiveOptionPanel(bool enabled)
    {
        if (enabled != _optionPanel.activeSelf)
        {
            _optionPanel.SetActive(enabled);
        }
    }

    public void SetActiveTutorialPanel(bool enalbed)
    {
        if(enalbed != _tutorialPanel.activeSelf)
        {
            _tutorialPanel.SetActive(enalbed);
        }
    }
    public IEnumerator TurnOn(float time)
    {
        float timer = 0f;

        while(timer < time)
        {
            _cutBorder.color = new Color(0, 0, 0, (time - timer) / time);
            yield return null;
            timer += Time.deltaTime;
        }
    }

    public IEnumerator TurnOff(float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            _cutBorder.color = new Color(0, 0, 0, timer / time);
            yield return null;
            timer += Time.deltaTime;
        }
    }

    public void SetCameraTransform(Vector3 basePos, Vector3 relativePos, Vector3 eulerAngle)
    {
        transform.position = basePos + relativePos;
        transform.eulerAngles = eulerAngle;
    }

    public void ResetRotation()
    {
        transform.eulerAngles = _originEuler;
    }

    public void CameraShake()
    {
        
    }
}
