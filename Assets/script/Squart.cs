using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;

public class Squart : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("UI Set")]
    public Image Excerciser;
    [LabeledArray(new string[] { "pos0", "pos1", "pos2" })]
    public Sprite[] ExceSpr;
    public Image Trainer;
    [LabeledArray(new string[] { "normal0", "more1" })]
    public Sprite[] TrainSpr;
    public Image OrderImg;
    [LabeledArray(new string[] { "SQUART", "ONEMORE" })]
    public Sprite[] OrderSpr;
    public TextMeshProUGUI OrderTxt;

    [Header("Audio Set")]
    public AudioSource GameSound;
    [LabeledArray(new string[] { "finish" })]
    public AudioClip[] SE;

    [Header("Control Variable")]
    public bool GameStart = false;
    public bool preSquartDone = false;

    private int squartCount;
    private int moreSquartCount;
    private int moreSquart;
    private float squartLength;
    private float nowSquartLength;
    private int prePos = 0;//0: pos0 / 1: pos1 / 2:pos2 
    private bool downPoint;
    private bool leaveAbove;
    private bool returnAbove;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.Squart);
        _ACT.Reset();
        _ACT.activeJoint[0] = true;
        _ACT.activeJoint[15] = true;
        _ACT.activeJoint[19] = true;
        _ACT.UpdateJoint();
        moreSquart = Random.Range(3, 5);
        squartCount = 0;
        moreSquartCount = 0;
        prePos = 0;
        downPoint = false;
        leaveAbove = false;
        returnAbove = false;
        preSquartDone = false;
        Trainer.sprite = TrainSpr[0];
        OrderImg.sprite = OrderSpr[0];
        Excerciser.sprite = ExceSpr[0];

    }

    public float get_Length()
    {
        Vector3 SpaineBasePos;
        Vector3 LeftFootPos;
        Vector3 RightFootPos;
        SpaineBasePos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.SpineBase].transform.position);
        LeftFootPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.FootLeft].transform.position);
        RightFootPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.FootRight].transform.position);
        Vector3 mid = (LeftFootPos + RightFootPos) / 2;
        return Mathf.Abs(mid.y - SpaineBasePos.y);
    }

    public void CheckCount(int pos)
    {
        if (prePos == pos)
        {
            return;
        }
        else
        {
            if (pos == 1)
            {
                leaveAbove = true;
            }
            else if (pos == 2)
            {
                downPoint = true;
            }
            else if (downPoint && pos == 0)
            {
                returnAbove = true;
            }
            prePos = pos;
        }
        if (leaveAbove && downPoint && returnAbove)
        {
            if (preSquartDone)
                moreSquartCount += 1;
            squartCount += 1;
            leaveAbove = false;
            downPoint = false;
            returnAbove = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            squartLength = get_Length();
            StartCoroutine("TrackingPos");
            StartCoroutine("TrainSquart");
        }
    }


    IEnumerator TrackingPos()
    {
        while (!GameStart)
        {
            //Debug.Log(FlagCheck());
            yield return new WaitForEndOfFrame();
            nowSquartLength = get_Length();
            if (nowSquartLength > (squartLength / 3) * 2)
            {
                CheckCount(0);
                Excerciser.sprite = ExceSpr[0];
            }
            else if (nowSquartLength <= (squartLength / 3) * 2 && nowSquartLength > (squartLength / 3))
            {
                CheckCount(1);
                Excerciser.sprite = ExceSpr[1];
            }
            else if (nowSquartLength <= (squartLength / 3))
            {
                CheckCount(2);
                Excerciser.sprite = ExceSpr[2];
            }
        }
    }

    IEnumerator TrainSquart()
    {
        OrderTxt.text = "스쿼트 5번!";
        yield return new WaitForSeconds(2f);
        while (squartCount != 5)
        {
            yield return new WaitForEndOfFrame();
            OrderTxt.text = squartCount.ToString() + " / 5";
        }
        squartCount = 0;
        OrderTxt.text = "0 / 1";
        preSquartDone = true;
        Trainer.sprite = TrainSpr[1];
        OrderImg.sprite = OrderSpr[1];
        while (moreSquartCount <= moreSquart)
        {
            yield return new WaitForEndOfFrame();
            if (squartCount != 0)
            {
                OrderTxt.text = "1 / 1";
                yield return new WaitForSeconds(0.3f);
                OrderTxt.text = "0 / 1";
                squartCount = 0;
            }
        }
        GameSound.PlayOneShot(SE[0]);
        _GM.GameFinish();
    }
}
