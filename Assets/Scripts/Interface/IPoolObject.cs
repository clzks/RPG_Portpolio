using UnityEngine;

public interface IPoolObject
{
    GameObject GetObject();
    string GetName();
    ObjectType GetObjectType();
    void Init();
    void ReturnObject();
}
