using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ActionPad : MonoBehaviour
{
    private DataManager _dataManager;
    [SerializeField]private List<ActionButton> actionButtonList;

    private void Awake()
    {
        _dataManager = DataManager.Get();
    }
    
    public ActionButton GetClickedButton()
    {
        var ActionButton = actionButtonList.FirstOrDefault(x => x.GetButtonDown() == true);
        
        if(null != ActionButton && string.Empty != ActionButton.GetActionName())
        {
            //actionName = ActionButton.GetActionName();
            return ActionButton;
        }
        else
        {
            return null;
        }
    }

    public void SetActionButton(int index, ActionInfo info)
    {
        Sprite sprite = null;
        if(0 == index || 4 == index)
        {
        
        }
        else
        {
            if(null == info)
            {
                actionButtonList[index].ResetAction();
            }
            else
            { 
                sprite = _dataManager.GetSkillImage(info.Name);
                actionButtonList[index].SetAction(info, sprite);
            }
        }
    }

    public ActionButton GetActionButton(int index)
    {
        if(index < actionButtonList.Count)
        {
            return actionButtonList[index];
        }
        else
        {
            return null;
        }
    }

    public void SetDragMode(bool isDragedSkill)
    {
        for(int i = 1; i < 4; ++i)
        {
            actionButtonList[i].isDragModeSetting = isDragedSkill;
        }
    }
}
