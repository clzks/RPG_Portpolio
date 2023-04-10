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
    QuestDestination,
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
    Arrive,
    Tutorial,
    Event,
    Count
}

public enum QuestProcessType
{
    NoneQuest,
    ReadyToQuest,
    Progress,
    Satisfy,
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

// �ó��������� ����Ʈ �ޱ� ���� ���� �� ��Ȳ�ۿ� ���� ��. ����Ʈ Ŭ���� �Ŀ��� �ٷ� ���� �ó������� �Ѿ���� �ó������� �ۼ��� ��
public enum ScenarioProcessType
{ 
    PrevQuest,          // ����Ʈ �ޱ� ��
    ProgressQuest,      // ����Ʈ ���� ��
    Count
}

public enum DialogType
{ 
    PrevDialog,
    ClearDialog,
    EventDialog,
    Count
}

public enum GameType
{ 
    NewGame,
    LoadGame,
    Count
}

public enum SaveType
{ 
    Possible,
    InBattle,
    InTutorial,
    InBoss,
    Count
}

public enum GameSettingType
{ 
    Homing,
    SkillDirection,
    FixStick,
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
        
        while(count <= 100)
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
    /// �þ߰� �ȿ� Ÿ���� �����ϴ°�? (�Ÿ� ����)
    /// </summary>
    /// <param name="actorDir">��ü�� �ٶ󺸴� ����</param>
    /// <param name="viewingAngle">�þ߰�</param>
    /// <param name="actorPosition">��ü�� ��ġ</param>
    /// <param name="targetPosition">�����ġ</param>
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

    public static EquipType ConvertItemTypeToEquipType(ItemType type)
    {
        switch (type)
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