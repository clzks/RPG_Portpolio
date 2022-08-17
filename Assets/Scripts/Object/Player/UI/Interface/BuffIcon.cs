using UnityEngine.UI;
using UnityEngine;

public class BuffIcon : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    public Image _buffImage;
    public Image _screen;

    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
        DonDestroy();
    }

    public string GetName()
    {
        return "";
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.BuffIcon;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(0, 0, 0);
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public void SetIconImage(Sprite sprite)
    {
        _buffImage.sprite = sprite;
    }

    public void SetFillAmount(float percent)
    {
        _screen.fillAmount = percent;
    }

    public void DonDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}
