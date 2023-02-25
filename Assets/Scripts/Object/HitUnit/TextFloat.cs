using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFloat : MonoBehaviour, IPoolObject
{
    private ObjectPoolManager _objectPool;
    private Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    private Vector3 originPos;
    private TextMeshPro _textMesh;
    private string _name = "DamageText";
    public float startPosY;
    public float breakPosY;
    public float destPosY;
    public float breakTime;
    public float stopTime;
    public float life;
    private float _tick = 0.016f;
    private WaitForSeconds _waitForTick;
    private WaitForSeconds _waitForBreak;
    private void Awake()
    {
        _waitForTick = new WaitForSeconds(_tick);
        _waitForBreak = new WaitForSeconds(stopTime);
        _objectPool = ObjectPoolManager.Get();
        _textMesh = GetComponent<TextMeshPro>();
        DonDestroy();
    }
    
    private IEnumerator FloatText()
    {
        float timer = 0f;
        var startPos = new Vector3(originPos.x, originPos.y + startPosY, originPos.z);
        var destPos = new Vector3(originPos.x, originPos.y + breakPosY, originPos.z);
        var destTime = breakTime;
        while(timer <= breakTime)
        {
            _textMesh.color += new Color(0, 0, 0, (_tick /breakTime) * 2f);
            Position = Vector3.Lerp(startPos, destPos, timer / destTime);
            timer += _tick;
            yield return _waitForTick;
        }

        startPos = destPos;
        destPos = new Vector3(originPos.x, originPos.y + destPosY, originPos.z);
        timer -= breakTime;
        destTime = life - breakTime;

        yield return _waitForBreak;

        while (timer <= life)
        {
            _textMesh.color -= new Color(0, 0, 0, (_tick / destTime) * 2f);
            Position = Vector3.Lerp(startPos, destPos, timer / destTime);
            timer += _tick;
            yield return _waitForTick;
        }

        ReturnObject();
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector3 GetPosition()
    {
        return Position;
    }

    public string GetName()
    {
        return _name;
    }

    public ObjectType GetObjectType()
    {
        return ObjectType.TextFloat;
    }

    public void ReturnObject()
    {
        //transform.position = new Vector3(5000, 5000, 5000);
        _objectPool.ReturnObject(this);
    }

    public void SetText(DamageTextType type, int damage, Vector3 pos)
    {
        switch (type)
        {
            case DamageTextType.Player:
                _textMesh.color = new Color(1, 0, 0, 0);
                break;

            case DamageTextType.Enemy:
                _textMesh.color = new Color(1, 1, 1, 0);
                break;

            case DamageTextType.Boss:
                _textMesh.color = new Color(1, 1, 1, 0);
                break;

            case DamageTextType.Shield:
                _textMesh.color = new Color(1, 1, 1, 0);
                break;

            case DamageTextType.Object:
                _textMesh.color = new Color(1, 1, 1, 0);
                break;
        }

        _textMesh.text = damage.ToString();
        Position = pos - new Vector3(0, 0, 3f);
        originPos = Position;
    }

    public void SetText(string descript, Vector3 pos)
    {
        _textMesh.color = new Color(0, 0, 0, 0);
        _textMesh.text = descript;
        Position = pos - new Vector3(0, 0, 3f);
        originPos = Position;
    }

    public void SetExpText(int value, Vector3 pos)
    {
        _textMesh.color = new Color(1, 0, 1, 0);

        _textMesh.text = "+ " + value.ToString() + " EXP";
        Position = pos - new Vector3(0, 0, 3f);
        originPos = Position;
    }

    public void SetLevelUpText(Vector3 pos)
    {
        _textMesh.color = new Color(1, 1, 0, 0);

        _textMesh.text = "LEVEL UP!";
        Position = pos - new Vector3(0, 0, 3f);
        originPos = Position;
    }

    public void SetTutorialMoveText(Vector3 pos)
    {
        _textMesh.color = new Color(1, 0, 0, 0);

        _textMesh.text = "튜토리얼 중에는 이동이 제한됩니다!";
        Position = pos - new Vector3(0, 0, 3f);
        originPos = Position;
    }

    public void SetTutorialLimitButtonText(Vector3 pos)
    { 

    }

    public void ExecuteFloat()
    {
        StartCoroutine(FloatText());
    }

    public void DonDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }
}