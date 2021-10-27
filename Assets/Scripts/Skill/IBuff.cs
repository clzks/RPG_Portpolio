using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    int GetId();
    void Update(float tick, IActor actor);
    void TakeActor(IActor actor);
    void Renew(IActor actor);
    //void SetActiveEffect(IActor actor, bool enabled);
}
