using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : IEquipment
{
    public int id;

    public void DropItem()
    {
        // 필드에 떨어지는 코드
    }

    public void GetItem()
    {
        // 인벤토리에 들어오는 코드
    }

    public void SellItem()
    {
        // 금? 을 얻는 코드
    }

    public void WearOff()
    {
        // 안장착
    }

    public void WearOn()
    {
        // 장착
    }
}
