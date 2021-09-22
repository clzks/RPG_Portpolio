using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    void Update(float tick, IActor actor);
    void TakeActor(IActor actor);
}
