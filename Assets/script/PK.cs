using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;

public class PK : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("UI Set")]
    public Image Kicker;
    [LabeledArray(new string[] { "Kick1", "Kick2" })]
    public Sprite[] Kicker_Spr;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject Ball;
    public Image[] ScoreBox;
    [LabeledArray(new string[] { "O", "X" })]
    public Sprite[] OX;
    [Header("Animation")]
    public Animator ballAnime;
    public AnimationClip[] ballAnis;
    [Header("Audio")]
    public AudioSource GameSound;
    [LabeledArray(new string[] { "whistle", "Kick" })]
    public AudioClip[] SESound;
    public bool GameStart;
    private int RouteNum;
    private int Score = 0;

    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.PK);
        _ACT.Reset();
        _ACT.activeJoint[7] = true;
        _ACT.activeJoint[11] = true;
        _ACT.UpdateJoint();
        ballAnime.Rebind();
        Kicker.sprite = Kicker_Spr[0];
        ScoreBox[0].enabled = false;
        ScoreBox[1].enabled = false;
        ScoreBox[2].enabled = false;
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            StartCoroutine("GloveWithHand");
            StartCoroutine("Shoot");
        }
    }
    public bool CheckBlock()
    {
        Vector3 BallPos = Ball.transform.position;
        Vector3 LeftPos = LeftHand.transform.position;
        Vector3 RightPos = RightHand.transform.position;
        float LeftDist = Vector2.Distance(new Vector2(LeftPos.x, LeftPos.y), (new Vector2(BallPos.x, BallPos.y)));
        float RightDist = Vector2.Distance(new Vector2(RightPos.x, RightPos.y), (new Vector2(BallPos.x, BallPos.y)));
        if (LeftDist <= 200f || RightDist <= 200f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            RouteNum = Random.Range(1, 10);
            GameSound.PlayOneShot(SESound[0]);
            yield return new WaitForSeconds(3.4f);
            ballAnime.SetInteger("choice", RouteNum);
            yield return new WaitForSeconds(0.5f);
            GameSound.PlayOneShot(SESound[1]);
            Kicker.sprite = Kicker_Spr[1];
            yield return new WaitForSeconds(ballAnis[RouteNum].length);
            if (CheckBlock())
            {
                ScoreBox[i].enabled = true;
                ScoreBox[i].sprite = OX[0];
                Score += 1;
            }
            else
            {
                ScoreBox[i].enabled = true;
                ScoreBox[i].sprite = OX[1];
            }
            yield return new WaitForSeconds(2f);
            ballAnime.Rebind();
            Kicker.sprite = Kicker_Spr[0];
        }
        if (Score >= 3)
        {
            _GM.GameFinish(false);
        }
        else
        {
            _GM.GameFinish(true);
        }
    }
    IEnumerator GloveWithHand()
    {
        Vector3 RightHandScreenPos;
        Vector3 LeftHandScreenPos;
        while (!GameStart)
        {
            //Debug.Log(FlagCheck());
            yield return new WaitForEndOfFrame();
            //if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] == null)
            //    break;
            RightHandScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.HandTipRight].transform.position);
            LeftHandScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.HandTipLeft].transform.position);
            LeftHand.transform.position = LeftHandScreenPos;
            RightHand.transform.position = RightHandScreenPos;
        }
    }
}
