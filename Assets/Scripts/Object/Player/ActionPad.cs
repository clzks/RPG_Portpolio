using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ActionPad : MonoBehaviour
{
    private DataManager _dataManager;
    public List<ActionButton> actionButtonList;

    private void Awake()
    {
        _dataManager = DataManager.Get();
    }
    
    public bool GetButtonDown(out string actionName)
    {
        var ActionButton = actionButtonList.FirstOrDefault(x => x.GetButtonDown() == true);
        
        if(null != ActionButton)
        {
            actionName = ActionButton.GetActionName();
            return true;
        }
        else
        {
            actionName = string.Empty;
            return false;
        }
    }
}
