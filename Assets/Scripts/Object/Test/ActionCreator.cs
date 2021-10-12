#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor.Animations;

public class ActionCreator : MonoBehaviour
{
    public GameObject hitUnitPrefab;
    public Animator mator;
    public AnimatorController _animController;
    private Text currSelectClipName;
    private Button selectPrevClip;
    private Button selectNextClip;
    private Button clipPlayButton;
    private Button cameraChangeButton;
    private Vector3 isoCameraVector = new Vector3(0f, 7.7f, -7f);
    private Vector3 orthoCameraVector = new Vector3(0f, 7.7f, 0f);
    //private List<AnimationClip> _clipInfoList;
    //private AnimationClip _currAnimatorClip;
    private List<string> _animationStateList;
    private string _currStateName;
    private int currIndex = 0;
    private bool isIso = true;
    public bool isCreate = true;
    public float normalizeTransitionDuration;
    public float normalizedTimeOffset;
    public float normalizedTransition;
    [Header("HitUnit")]
    public List<HitUnitInfo> hitUnitList;

    private void Awake()
    {
        _animController = mator.runtimeAnimatorController as AnimatorController;
        //_clipInfoList = _animController.animationClips.ToList();
        //_currAnimatorClip = _clipInfoList[currIndex];
        _animationStateList = GetAnimationStateList();
        _currStateName = _animationStateList[currIndex];
        selectPrevClip = GameObject.Find("Canvas/SelectPrevClip").GetComponent<Button>();
        selectNextClip = GameObject.Find("Canvas/SelectNextClip").GetComponent<Button>();
        clipPlayButton = GameObject.Find("Canvas/TestButton").GetComponent<Button>();
        cameraChangeButton = GameObject.Find("Canvas/CameraChange").GetComponent<Button>();
        currSelectClipName = GameObject.Find("Canvas/ClipName").GetComponent<Text>();
        UpdateSelectClipName();
        selectPrevClip.onClick.AddListener(OnClickSelectPrevClipButton);
        selectNextClip.onClick.AddListener(OnClickSelectNextClipButton);
        clipPlayButton.onClick.AddListener(OnClickPlayClipButton);
        cameraChangeButton.onClick.AddListener(OnClickChangeCameraButton);
    }

    void Update()
    {
        var moveDir = new Vector3(0, 0, 0);
        var isMove = false;

        if(Input.GetKey(KeyCode.W))
        {
            moveDir += new Vector3(0, 0, 1);
            isMove = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir += new Vector3(0, 0, -1);
            isMove = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir += new Vector3(-1, 0, 0);
            isMove = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir += new Vector3(1, 0, 0);
            isMove = true;
        }

        if (true == isMove)
        {
            transform.forward = moveDir.normalized;
            transform.position += moveDir.normalized * 3f * Time.deltaTime;
        }
    }

    private void OnClickSelectPrevClipButton()
    {
        if(currIndex == 0)
        {
            currIndex = _animationStateList.Count - 1;
        }
        else
        {
            currIndex--;
        }

        _currStateName = _animationStateList[currIndex];
        UpdateSelectClipName();
    }

    private void OnClickSelectNextClipButton()
    {
        if (currIndex == _animationStateList.Count - 1)
        {
            currIndex = 0;
        }
        else
        {
            currIndex++;
        }

        _currStateName = _animationStateList[currIndex];
        UpdateSelectClipName();
    }

    private void UpdateSelectClipName()
    {
        currSelectClipName.text = _currStateName;
    }

    private void OnClickPlayClipButton()
    {
        //mator.Play(_currStateName, )

        mator.CrossFade(_currStateName, normalizeTransitionDuration, 0, 0f);
        if (true == isCreate)
        {
            for (int i = 0; i < hitUnitList.Count; ++i)
            {
                //MakeSampleHitUnit(i);
            }
        }
    }

    private void OnClickChangeCameraButton()
    {
        isIso = !isIso;
        var cam = Camera.main;
        if(true == isIso)
        {
            cam.transform.rotation = Quaternion.Euler(45f, 0, 0);
            cam.transform.position = isoCameraVector;
        }
        else
        {
            cam.transform.rotation = Quaternion.Euler(90f, 0, 0);
            cam.transform.position = orthoCameraVector;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.0f, 0f), 0.4f);
    }

    public List<string> GetAnimationStateList()
    {
        AnimatorControllerLayer[] allLayer = _animController.layers;

        ChildAnimatorState[] states = allLayer[0].stateMachine.states;
        var list = new List<string>();
        for(int i = 0; i < states.Length; ++i)
        {
            list.Add(states[i].state.name);
        }
        list = list.OrderBy(x => x).ToList();
        return list;
    }

    public void MakeHitUnit(int index)
    {
        if(hitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = Instantiate(hitUnitPrefab).GetComponent<HitUnit>();
        HitUnitInfo info = hitUnitList[index];
        hitUnit.SetSampleHitUnit(info, transform);
    }

    public void MakeSampleHitUnit(int index)
    {
        if (hitUnitList.Count <= index)
        {
            return;
        }
        HitUnit hitUnit = Instantiate(hitUnitPrefab).GetComponent<HitUnit>();
        HitUnitInfo info = hitUnitList[index];
        hitUnit.SetSampleHitUnit(info, transform);
    }
}
#endif
