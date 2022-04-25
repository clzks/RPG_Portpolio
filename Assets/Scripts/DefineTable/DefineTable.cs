using UnityEngine;
using UnityEngine.AI;

public enum ObjectType
{ 
    HitUnit,
    Player,
    Enemy,
    Effect,
    Map,
    TextFloat,
    GroundItem,
    InventoryIcon,
    BuffIcon,
    RewardIcon,
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
    Roll,
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
    Attack,
    AttackSpeed,
    Speed,
    Dot,
    Shield,
    Stun,
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

public enum QuestType
{ 
    Item,
    Kill,
    LevelUp,
    Gold,
    Count
}

public enum QuestProcessType
{
    NoneQuest,
    Progress,
    SatisFy,
    Count
}

public enum RewardType
{ 
    Item,
    Exp,
    Gold,
    Stat,
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
        
        // 
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

    /// <summary>
    /// 시야각 안에 타겟이 존재하는가? (거리 제외)
    /// </summary>
    /// <param name="actorDir">주체가 바라보는 방향</param>
    /// <param name="viewingAngle">시야각</param>
    /// <param name="actorPosition">주체의 위치</param>
    /// <param name="targetPosition">대상위치</param>
    /// <returns></returns>
    public static bool IsTargetInSight(Vector3 actorDir, float viewingAngle, Vector3 actorPosition, Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - actorPosition).normalized;
        float dot = Vector3.Dot(actorDir, dir);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        
        if(angle <= viewingAngle)
        {
            return true;
        }

        return false;
    }

    public static EquipType ConvertItemTypeToEquipType(ItemType iType)
    {
        switch (iType)
        {
            case ItemType.Weapon:
                return EquipType.Weapon;
            case ItemType.Armor:
                return EquipType.Armor;
            case ItemType.Accessory:
                return EquipType.Accessory;
            case ItemType.Quest:
            case ItemType.Consumable:
            case ItemType.Count:
            default:
                return EquipType.Count;
        }
    }
}