using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour, IPoolObject
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
        return ObjectType.DamageText;
    }

    public void ReturnObject()
    {
        //_textMesh.color = new Color(1, 1, 1, 0);
        transform.position = new Vector3(5000, 5000, 5000);
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

    public void ExecuteFloat()
    {
        StartCoroutine(FloatText());
    }
}