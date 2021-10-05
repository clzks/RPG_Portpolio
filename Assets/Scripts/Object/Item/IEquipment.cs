using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipment : IItem
{
    void WearOn();
    void WearOff();
}
