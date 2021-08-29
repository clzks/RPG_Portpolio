using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTest : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material normal;
    public Material trans;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(meshRenderer.material == normal)
            {
                meshRenderer.material = trans;
            }
            else
            {
                meshRenderer.material = normal;
            }
        }
    }
}
