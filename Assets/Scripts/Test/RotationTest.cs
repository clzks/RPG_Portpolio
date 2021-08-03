using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Transform focus;
    public Vector3 posFromFocus;

    private void Awake()
    {
        transform.position = focus.position + posFromFocus;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(focus.position, new Vector3(0, 1, 0), 1f);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(focus.position, new Vector3(0, 1, 0), -1f);
        }
        
        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.position = focus.position + posFromFocus;
            transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 1, 0));
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            float f;
            Vector3 v = Vector3.zero;
            transform.rotation.ToAngleAxis(out f, out v);
            Debug.Log(f + "/" + v);
        }
    }
}
