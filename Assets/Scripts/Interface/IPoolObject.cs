using UnityEngine;

public interface IPoolObject
{
    GameObject GetObject();
    Vector3 GetPosition();
    string GetName();
    ObjectType GetObjectType();
    void ReturnObject();
}
