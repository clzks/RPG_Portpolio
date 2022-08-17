using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardIcon : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    public Image rewardImage;
    public Text count;
    public Text descript;

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
        return ObjectType.RewardIcon;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(0, 0, 0);
    }

    public void ReturnObject()
    {
        _objectPool.ReturnObject(this);
    }

    public void SetIcon(Sprite sp, string countText, string description)
    {
        rewardImage.sprite = sp;
        count.text = "x " + countText;
        descript.text = description;
    }

    public void DonDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}
