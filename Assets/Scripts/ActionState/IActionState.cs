using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionState 
{
    void Enter();
    IActionState Update();
    IActionState ChangeState(IActionState state);
    void Exit();
}
