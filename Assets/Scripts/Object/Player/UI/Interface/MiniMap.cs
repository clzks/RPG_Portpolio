using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiniMap : MonoBehaviour
{
    public Camera _camera;
    public MeshRenderer meshRenderer;

    public float ScreenWidth = 17.77f;
    public float ScreenHeight = 12.20776f;
    //public float Y = -8f;

    private void OnEnable()
    {
        meshRenderer.material.mainTextureScale = new Vector2(ScreenWidth * 0.01f, ScreenWidth * 0.01f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CheckDifferent();
        }
    }

    public void MiniMapUpdate(Vector4[] enemyPosArray, Vector4 PlayerPos)
    {
        var CameraBasicX = ScreenWidth / 2f;
        var CameraBasicY = -13f;
        var offset = new Vector2(_camera.transform.position.x - CameraBasicX, _camera.transform.position.z - CameraBasicY) * 0.01f;
        meshRenderer.material.mainTextureOffset = offset;
        meshRenderer.material.SetVector("_PlayerPos", PlayerPos);
        meshRenderer.material.SetVectorArray("_EnemyPosArray", enemyPosArray);
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
        RaycastHit ccRayHit;

        Vector3 cameraDir = _camera.transform.localRotation * Vector3.forward;
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0, 0, 0)), cameraDir, out lbRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(1, 0, 0)), cameraDir, out rbRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0, 1, 0)), cameraDir, out luRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(1, 1, 0)), cameraDir, out ruRayHit);
        Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), cameraDir, out ccRayHit);

        Debug.Log(lbRayHit.point);
        Debug.Log(rbRayHit.point);
        Debug.Log(luRayHit.point);
        Debug.Log(ruRayHit.point);
        Debug.Log(ccRayHit.point);
        Debug.Log(_camera.transform.position);
        
        Debug.Log(rbRayHit.point.x - lbRayHit.point.x);
        Debug.Log(ruRayHit.point.z - lbRayHit.point.z);
    }
}
