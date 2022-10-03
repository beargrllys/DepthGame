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
    public Tutorial _TUTO;

    public bool testVal = false;
    public GameObject Img;
    private Image Img_opt;
    private bool bodyDetect;
    //개발의 편의성을 위해 관절 부위 배열에 명칭 매칭
    [LabeledArray(new string[] { "SpineBase_0", "Neck_1", "SpineMid_2", "Head_3", "ShoulderLeft_4", "ElbowLeft_5", "WristLeft_6", "HandLeft_7", "ShoulderRight_8", "ElbowRight_9", "WristRight_10", "HandRight_11", "HipLeft_12", "KneeLeft_13", "AnkleLeft_14", "FootLeft_15", "HipRight_16", "KneeRight_17", "AnkleRight_18", "FootRight_19", "SpineShoulder_20", "HandTipLeft_21", "ThumbLeft_22", "HandTipRight_23", "ThumbRight_24" })]
    public GameObject[] _BoneMap = new GameObject[25];

    [LabeledArray(new string[] { "SpineBase_0", "Neck_1", "SpineMid_2", "Head_3", "ShoulderLeft_4", "ElbowLeft_5", "WristLeft_6", "HandLeft_7", "ShoulderRight_8", "ElbowRight_9", "WristRight_10", "HandRight_11", "HipLeft_12", "KneeLeft_13", "AnkleLeft_14", "FootLeft_15", "HipRight_16", "KneeRight_17", "AnkleRight_18", "FootRight_19", "SpineShoulder_20", "HandTipLeft_21", "ThumbLeft_22", "HandTipRight_23", "ThumbRight_24" })]
    public bool[] activeJoint = new bool[25];

    /* Start Game Variable*/
    [Header("Game Lobby")]
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject TutoBtn;
    public Sprite[] CountDown = new Sprite[5];
    public Image CountNum;
    public GameObject GazyBar;
    public GameObject Gazy;

    void Start()
    {
        Img_opt = Img.GetComponent<Image>();
    }
    
    //트래킹 관절 부위 - 게임오브젝트 출동 여부 확인 
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
        bool TutoCond = false;
        bool PassStep = false;
        float WaitingT = 0f;
        float std = 6.0f;
        //시작시 양손이 시작지점에 있는지 
        while (!PassStep)
        {
            yield return new WaitForEndOfFrame();
            if (_BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                cond1 = judgeByGameObject((int)Kinect.JointType.HandTipRight, RightHand);
                cond2 = judgeByGameObject((int)Kinect.JointType.HandTipLeft, LeftHand);
                TutoCond = judgeByGameObject((int)Kinect.JointType.HandTipRight, TutoBtn);
                if (TutoCond)
                {
                    float G_scale;
                    GazyBar.SetActive(true);
                    WaitingT += Time.deltaTime;
                    if (WaitingT < 3.0f)
                    {
                        G_scale = WaitingT / 3.0f;
                        Gazy.transform.localScale = new Vector3(G_scale, 1, 1);
                    }
                    else
                    {
                        _UIM.UISetOff((int)UIset.SetName.Intro);
                        _UIM.UISetOn((int)UIset.SetName.Tutorial);
                        _TUTO.Setting();
                    }
                }
                else
                {
                    GazyBar.SetActive(false);
                    Gazy.transform.localScale = new Vector3(0, 1, 1);
                }
                if (cond1 && cond2)
                {
                    WaitingT += Time.deltaTime;
                    if (WaitingT <= 1.0f)
                    {
                        CountNum.enabled = true;
                        CountNum.sprite = CountDown[4];
                    }
                    else if (WaitingT <= 2.0f)
                    {
                        CountNum.sprite = CountDown[3];
                    }
                    else if (WaitingT <= 3.0f)
                    {
                        CountNum.sprite = CountDown[2];
                    }
                    else if (WaitingT <= 4.0f)
                    {
                        CountNum.sprite = CountDown[1];
                    }
                    else if (WaitingT <= 5.0f)
                    {
                        CountNum.sprite = CountDown[0];
                    }
                    else if (WaitingT <= std)
                    {
                        PassStep = true;
                        _UIM.UISetOff((int)UIset.SetName.Intro);
                        _GM.init_start = true;
                    }
                }
                if (!cond1 && !cond2 && !TutoCond)
                {
                    WaitingT = 0f;
                    CountNum.enabled = false;
                }
            }
        }

    }
}

