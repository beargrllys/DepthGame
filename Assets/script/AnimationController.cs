using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class AnimationController : MonoBehaviour //게임진행을 위한 UI와 BodyTracking 간의 선택 조건 감지 / 콘텐츠 진행관련 코드 포함
{
    public BodyTracker _BT;
    public UIManager _UIM;
    public BodySourceView _BSV;
    public GameManage _GM;

    public bool testVal = false;
    public GameObject Img;
    private Image Img_opt;
    private bool bodyDetect;
    [LabeledArray(new string[] { "SpineBase_0", "Neck_1", "SpineMid_2", "Head_3", "ShoulderLeft_4", "ElbowLeft_5", "WristLeft_6", "HandLeft_7", "ShoulderRight_8", "ElbowRight_9", "WristRight_10", "HandRight_11", "HipLeft_12", "KneeLeft_13", "AnkleLeft_14", "FootLeft_15", "HipRight_16", "KneeRight_17", "AnkleRight_18", "FootRight_19", "SpineShoulder_20", "HandTipLeft_21", "ThumbLeft_22", "HandTipRight_23", "ThumbRight_24" })]
    public GameObject[] _BoneMap = new GameObject[25];

    [LabeledArray(new string[] { "SpineBase_0", "Neck_1", "SpineMid_2", "Head_3", "ShoulderLeft_4", "ElbowLeft_5", "WristLeft_6", "HandLeft_7", "ShoulderRight_8", "ElbowRight_9", "WristRight_10", "HandRight_11", "HipLeft_12", "KneeLeft_13", "AnkleLeft_14", "FootLeft_15", "HipRight_16", "KneeRight_17", "AnkleRight_18", "FootRight_19", "SpineShoulder_20", "HandTipLeft_21", "ThumbLeft_22", "HandTipRight_23", "ThumbRight_24" })]
    public bool[] activeJoint = new bool[25];

    /* Start Game Variable*/
    [Header("Game Lobby")]
    public GameObject RightHand;
    public GameObject LeftHand;

    void Start()
    {
        Img_opt = Img.GetComponent<Image>();
    }

    public bool judgeByGameObject(int JointNum, GameObject UIImg)
    {
        Vector2 sizeset = new Vector3(Img_opt.rectTransform.sizeDelta.x, Img_opt.rectTransform.sizeDelta.y);
        Vector3 scaleset = new Vector3(Img.transform.localScale.x, Img.transform.localScale.y, Img.transform.localScale.z);
        return _BT.jointTo2D_judge(_BoneMap[JointNum].transform.position, UIImg.transform.position, sizeset, scaleset);
    }

    // Update is called once per frame
    void Update()
    {
        if (_BoneMap[(int)Kinect.JointType.SpineBase] != null && bodyDetect == false)
        {
            bodyDetect = true;
            //Debug.Log(judgeByGameObject((int)Kinect.JointType.HandTipRight, Img));
            StartCoroutine("GameStart");
        }
    }

    public void Reset()
    {
        for (int jt = (int)Kinect.JointType.SpineBase; jt <= (int)Kinect.JointType.ThumbRight; jt++)
            activeJoint[jt] = false;
    }
    public void UpdateJoint()
    {
        for (int jt = (int)Kinect.JointType.SpineBase; jt <= (int)Kinect.JointType.ThumbRight; jt++)
        {
            if (activeJoint[jt])
            {
                _BoneMap[jt].GetComponent<Renderer>().material = _BSV.UsingPoint;
            }
            else
            {
                _BoneMap[jt].GetComponent<Renderer>().material = _BSV.Transparent;
            }
        }
    }

    IEnumerator GameStart()
    {
        bool cond1 = false;//오른손 아이콘에 오른손
        bool cond2 = false;//왼손 아이콘에 왼손
        bool PassStep = false;
        float WaitingT = 0f;
        float std = 5.0f;
        while (!PassStep)
        {
            yield return new WaitForEndOfFrame();
            if (_BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                cond1 = judgeByGameObject((int)Kinect.JointType.HandTipRight, RightHand);
                cond2 = judgeByGameObject((int)Kinect.JointType.HandTipLeft, LeftHand);
                //Debug.Log("cond1" + cond1 + " / " + "cond2" + cond2);
                if (cond1 && cond2)
                {
                    WaitingT += Time.deltaTime;
                    if (WaitingT >= std)
                    {
                        PassStep = true;
                        _UIM.UISetOff((int)UIset.SetName.Intro);
                        _GM.init_start = true;
                    }
                }
            }
        }

    }
}

