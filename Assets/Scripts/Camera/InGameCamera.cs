using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private Vector3 _cameraDistance = new Vector3(0, 30f, -20.1f);
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    private float _limitR = 90f;
    private float _limitL = 9f;
    private float _limitU = 70f;
    private float _limitB = -6f;

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

    public void CameraShake()
    {

    }
}
