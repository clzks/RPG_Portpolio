using UnityEngine;
using UnityEngine.AI;

public enum ObjectType
{ 
    HitUnit,
    Player,
    Enemy,
    Effect,
    Map,
    DamageText,
    GroundItem,
    Count
}


public enum SceneType
{
    Loading,
    Title,
    Lobby,
    InGame,
    Count
}

public enum MapEventType
{
    SummonPoint,
    Transition,
    UniqueMonster,
    NormalMonster,
    Count
}

public enum ActionType
{
    NormalAttack,
    Skill,
    Count
}

public enum DamageTextType
{ 
    Player,
    Enemy,
    Boss,
    Shield,
    Object,
    Count
}

public enum CalculateType
{ 
    Add,
    Multiply,
    Count
}

public enum BuffType
{ 
    Damage,
    AttackSpeed,
    Speed,
    Dot,
    Count
}

public enum EquipType
{ 
    Weapon,
    Armor,
    Accessory,
    Count
}

public enum ItemType
{
    Weapon,
    Armor,
    Accessory,
    Quest,
    Consumable,
    Count
}

public enum ItemClassType
{ 
    Normal,
    Unique,
    Epic,
    Legendary,
    Count
}

public enum GroundItemType
{ 
    Gold,
    Buff,
    Item,
    Count
}


public static class Formula
{
    public static Vector3 BezierMove(Vector3 start, Vector3 p, Vector3 dest, float t)
    {
        Vector3 first = Vector3.Lerp(start, p, t);
        Vector3 second = Vector3.Lerp(p, dest, t);

        return Vector3.Lerp(first, second, t);
    }

    public static Vector3 GetRandomPatrolPosition(Vector3 startPos, float range)
    {
        NavMeshHit navHit;
        int count = 0;
        Vector3 randomPoint = startPos + Random.insideUnitSphere * range;
        randomPoint.y = startPos.y;
        var result = Vector3.zero;
        while(count <= 200)
        {
            count++;
            result = startPos;
            
            if(true == (NavMesh.SamplePosition(randomPoint, out navHit, 0.1f, NavMesh.AllAreas)))
            {
                result = navHit.position;
                break;
            }
        }

        return result;
    }
}