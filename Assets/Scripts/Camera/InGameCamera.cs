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
