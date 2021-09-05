using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiniMap : MonoBehaviour
{
    public Camera _camera;
    public MeshRenderer meshRenderer;

    public float screenWidth = 17.77f;
    public float screenHeight = 12.20776f;


    private void OnEnable()
    {
        meshRenderer.material.mainTextureScale = new Vector2(screenWidth * 0.01f, screenWidth * 0.01f);
    }

    public void MiniMapUpdate()
    {
        var CameraBasicX = screenWidth / 2f;    
        var CameraBasicY = screenHeight / 2f;
        var offset = new Vector2(_camera.transform.position.x - CameraBasicX, _camera.transform.position.z - CameraBasicY) * 0.01f;
        meshRenderer.material.mainTextureOffset = offset;
    }

    private void LateUpdate()
    {
        MiniMapUpdate();
    }

   public void CheckScreenWidth()
   {
       RaycastHit rayHit;
       Vector3 cameraDir = _camera.transform.localRotation * Vector3.forward;
   
       if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0,0,0)), cameraDir, out rayHit))
       {
           var rayHitPos = rayHit.transform.position;
   
           var leftBottom = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
           var topRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
   
           float screenWidth = topRight.x - leftBottom.x;
           float screenHeight = topRight.z - leftBottom.z;
   
           Debug.Log(screenWidth + " : 가로크기");
           Debug.Log(screenHeight + " : 세로크기");
       }
   }

    public void CheckDifferent()
    {
        RaycastHit lbRayHit;
        RaycastHit rbRayHit;
        RaycastHit luRayHit;
        RaycastHit ruRayHit;

        Vector3 cameraDir = _camera.transform.localRotation * Vector3.forward;
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0, 0, 0)), cameraDir, out lbRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(1, 0, 0)), cameraDir, out rbRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0, 1, 0)), cameraDir, out luRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(1, 1, 0)), cameraDir, out ruRayHit);

        Debug.Log(lbRayHit.point);
        Debug.Log(rbRayHit.point);
        Debug.Log(luRayHit.point);
        Debug.Log(ruRayHit.point);

        Debug.Log(rbRayHit.point.x - lbRayHit.point.x);
        Debug.Log(ruRayHit.point.z - lbRayHit.point.z);
    }
}
