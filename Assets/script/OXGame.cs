using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;

public class OXGame : MonoBehaviour
{

    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;

    [Header("UI Set")]
    public TextMeshProUGUI QuizBox;
    public Image DocPic;
    public Sprite DocNormalPic;
    public Sprite DocWrongPic;
    public GameObject Oman;
    public GameObject Xman;
    Vector3 OmanPos;
    Vector3 XmanPos;

    [Header("UserData")]
    public int choice;//-1:X  0:null  1:O
    public string[] quizlist;

    [Header("Audio Set")]
    public AudioSource GameSound;
    [LabeledArray(new string[] { "Correct", "Wrong" })]
    public AudioClip[] GameSE;
    public AudioClip[] QuizClip;
    public int[] Quiz_ans;

    public bool GameStart;

    void Start()
    {
        OmanPos = Oman.transform.position;
        XmanPos = Xman.transform.position;
    }

    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.OXGame);
        QuizBox.text = "";
        _ACT.Reset();
        _ACT.activeJoint[1] = true;
        _ACT.UpdateJoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            StartCoroutine("QuizInHand");
            StartCoroutine(QuizGameManager());
        }
    }

    IEnumerator QuizInHand()
    {
        Vector3 NeckScreenPos;
        while (!GameStart)
        {
            //Debug.Log(FlagCheck());
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] == null)
                break;
            NeckScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.Neck].transform.position);
            if (NeckScreenPos.x < 750)
            {
                Oman.transform.position = new Vector3(OmanPos.x, OmanPos.y + 70f, OmanPos.z);
                choice = 1;
            }
            else
            {
                Oman.transform.position = new Vector3(OmanPos.x, OmanPos.y, OmanPos.z);
            }
            if (NeckScreenPos.x > 1170)
            {
                Xman.transform.position = new Vector3(XmanPos.x, XmanPos.y + 70f, XmanPos.z);
                choice = -1;
            }
            else
            {
                Xman.transform.position = new Vector3(XmanPos.x, XmanPos.y, XmanPos.z);
            }
            if (NeckScreenPos.x <= 1170 && NeckScreenPos.x >= 750)
                choice = 0;

        }
    }

    IEnumerator QuizGameManager()
    {
        int randval = 0;
        randval = Random.Range(0, quizlist.Length);
        QuizBox.text = quizlist[randval];
        GameSound.PlayOneShot(QuizClip[randval]);
        yield return new WaitForSeconds(QuizClip[randval].length);
        _GM.ChangeTitle(1f, "3");
        yield return new WaitForSeconds(1f);
        _GM.ChangeTitle(1f, "2");
        yield return new WaitForSeconds(1f);
        _GM.ChangeTitle(1f, "1");
        yield return new WaitForSeconds(1f);
        if (Quiz_ans[randval] == choice)
        {
            QuizBox.text = "Good!";
            GameSound.PlayOneShot(GameSE[0]);
            yield return new WaitForSeconds(1.0f);
            _GM.GameFinish(false);
        }
        else if (choice == 0)
        {
            DocPic.sprite = DocWrongPic;
            QuizBox.text = "Times up!";
            GameSound.PlayOneShot(GameSE[1]);
            yield return new WaitForSeconds(1.0f);
            DocPic.sprite = DocNormalPic;
            _GM.GameFinish(true);
        }
        else
        {
            DocPic.sprite = DocWrongPic;
            QuizBox.text = "Wrong!";
            GameSound.PlayOneShot(GameSE[1]);
            yield return new WaitForSeconds(1.0f);
            DocPic.sprite = DocNormalPic;
            _GM.GameFinish(true);
        }
    }
}
