using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private Vector3 _cameraDistance = new Vector3(0, 30f, -21f);

    public void SetCameraDistance(Vector3 playerPos)
    {
        //_cameraDistance = transform.position - playerPos;
    }

    public void FollowPlayer(Vector3 playerPos)
    {
        transform.position = playerPos + _cameraDistance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            _cameraDistance += new Vector3(0, 0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            _cameraDistance += new Vector3(0, 0, 1);
        }
    }

    public void CameraShake()
    {

    }
}
