using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    int GetId();
    string GetName();
    void Update(float tick, IActor actor);
    void TakeActor(IActor actor);
    void Renew(IActor actor);
    void SetBuffIcon(BuffIcon buffIcon, Sprite sprite);
    //void SetActiveEffect(IActor actor, bool enabled);
}
