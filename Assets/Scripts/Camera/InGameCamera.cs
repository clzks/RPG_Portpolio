using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private Vector3 _cameraDistance;

    public void SetCameraDistance(Vector3 playerPos)
    {
        _cameraDistance = transform.position - playerPos;
    }

    public void FollowPlayer(Vector3 playerPos)
    {
        transform.position = playerPos + _cameraDistance;
    }
    

    public void CameraShake()
    {

    }
}
