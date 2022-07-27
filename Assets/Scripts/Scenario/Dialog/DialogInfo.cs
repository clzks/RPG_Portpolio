using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInfo : IData
{
    public int Id;
    public string Name;
    public List<Dialog> DialogList;
    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }
}
