using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class AvoidPunch : MonoBehaviour
{

    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;

    [Header("UI Set")]
    public GameObject ZamMin;
    public GameObject WaitPunch;
    public GameObject[] ColoredPunch = new GameObject[3];
    public GameObject AvoidTxt;
    public GameObject CountdownImg;
    public Sprite[] Countdown = new Sprite[3];
    public GameObject SepLine;
    public GameObject RedShield;
    public GameObject YourChar;
    public GameObject AvoidFail;
    public GameObject AvoidSucess;
    private Image countImg;

    [Header("Audio Set")]
    public AudioSource GameSound;
    [LabeledArray(new string[] { "ZamMin0", "ZamMin1", "ZamMin2", "Punch", "finish", "count" })]
    public AudioClip[] SE;

    [Header("Control Variable")]
    public bool GameStart = false;
    private int[,] punchCase = new int[12, 3] { { 1, 2, 3 }, { 1, 3, 2 }, { 2, 1, 3 }, { 2, 3, 1 }, { 3, 1, 2 }, { 3, 2, 1 }, { 2, 1, 2 }, { 1, 2, 1 }, { 3, 2, 3 }, { 2, 3, 2 }, { 3, 1, 3 }, { 1, 3, 1 } };
    private int[] choiced = new int[3];
    private int punchNum = 0;
    private bool keepgo = true;
    private int ZamminSaid = 0;
    private IEnumerator[] Corutines = new IEnumerator[3];
    // Start is called before the first frame update
    void Start()
    {
        countImg = CountdownImg.GetComponent<Image>();
    }

    public void Setting()
    {
        int idx;
        _UIM.UISetOn((int)UIset.SetName.AvoidPunch);
        _ACT.Reset();
        _ACT.activeJoint[3] = true;
        _ACT.UpdateJoint();
        idx = Random.Range(0, 12);
        choiced[0] = punchCase[idx, 0];
        choiced[1] = punchCase[idx, 1];
        choiced[2] = punchCase[idx, 2];
        keepgo = true;
        punchNum = 0;
        ZamminSaid = Random.Range(0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            GameSound.PlayOneShot(SE[ZamminSaid]);
            StartCoroutine(AvoidGameManager());
            StartCoroutine("TrackingArea");
        }
    }

    public bool CheckPos(int ans)
    {//true 성공, false 실패
        int chk = -1;
        if (YourChar.transform.position.x < 658f)
        {
            chk = 0;
        }
        else if (YourChar.transform.position.x > 1265f)
        {
            chk = 2;
        }
        else
        {
            chk = 1;
        }
        if (ans != chk)
        {
            return true;
        }
        else
        {
            keepgo = false;
            return false;
        }

    }

    public void stopCor()
    {
        if (Corutines[0] != null)
        {
            StopCoroutine(Corutines[0]);
        }
        if (Corutines[1] != null)
        {
            StopCoroutine(Corutines[1]);
        }
        if (Corutines[2] != null)
        {
            StopCoroutine(Corutines[2]);
        }
    }
    public void reset(bool HeartDown)
    {
        SepLine.SetActive(false);
        RedShield.SetActive(false);
        ZamMin.SetActive(true);
        YourChar.SetActive(true);
        AvoidTxt.SetActive(true);
        CountdownImg.SetActive(false);
        AvoidSucess.SetActive(false);
        AvoidFail.SetActive(false);
        stopCor();
        StopAllCoroutines();
        _GM.GameFinish(HeartDown);
    }

    public void result(bool sucessOrFail)
    {
        if (sucessOrFail == false)
        {//실패했을경우
            GameSound.PlayOneShot(SE[3]);
            StopCoroutine("ShowResult");
            StopCoroutine("TrackingArea");
            StopCoroutine("AvoidGameManager");
            stopCor();
            StartCoroutine(ShowResult(false));
        }
        else
        {
            if (punchNum == 0)
            {
                punchNum += 1;
            }
            else if (punchNum == 1)
            {
                punchNum += 1;
            }
            else if (punchNum == 2)
            {
                GameSound.PlayOneShot(SE[4]);
                StartCoroutine(ShowResult(true));
            }
        }
    }
    IEnumerator ShowResult(bool result)
    {
        SepLine.SetActive(false);
        RedShield.SetActive(false);
        ZamMin.SetActive(false);
        YourChar.SetActive(false);
        ColoredPunch[0].transform.localScale = new Vector3(1, 1, 1);
        ColoredPunch[1].transform.localScale = new Vector3(1, 1, 1);
        ColoredPunch[2].transform.localScale = new Vector3(1, 1, 1);
        ColoredPunch[0].SetActive(false);
        ColoredPunch[1].SetActive(false);
        ColoredPunch[2].SetActive(false);
        if (result)
        {
            AvoidSucess.SetActive(true);
            yield return new WaitForSeconds(1f);
            reset(false);
        }
        else
        {
            AvoidFail.SetActive(true);
            yield return new WaitForSeconds(1f);
            reset(true);
        }
    }

    IEnumerator TrackingArea()
    {
        Vector3 HeadScreenPos;
        while (!GameStart)
        {
            //Debug.Log(FlagCheck());
            yield return new WaitForEndOfFrame();
            //if (_ACT._BoneMap[(int)Kinect.JointType.Head] == null)
            //    break;
            HeadScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.Head].transform.position);
            YourChar.transform.position = new Vector3(HeadScreenPos.x, YourChar.transform.position.y, YourChar.transform.position.z);
        }

    }

    IEnumerator AvoidGameManager()
    {
        /*for (int i = 0; i < 3; i++)
        {
            countImg.sprite = Countdown[i];
            yield return new WaitForSeconds(0.4f);
        }*/
        AvoidTxt.SetActive(false);
        CountdownImg.SetActive(false);
        SepLine.SetActive(true);
        if (keepgo)
        {
            Corutines[0] = PunchManager(0);
            StartCoroutine(PunchManager(0));
            yield return new WaitForSeconds(0.8f);
        }
        if (keepgo)
        {
            Corutines[1] = PunchManager(1);
            StartCoroutine(PunchManager(1));
            yield return new WaitForSeconds(0.8f);
        }
        if (keepgo)
        {
            Corutines[2] = PunchManager(2);
            StartCoroutine(PunchManager(2));
            yield return new WaitForSeconds(0.8f);
        }

    }

    IEnumerator PunchManager(int puchCode)
    {
        bool toggle = true;
        float interval = 0.01f;
        for (int i = 0; i < 6 && keepgo; i++)
        {//1.2초 대기
            ColoredPunch[choiced[puchCode] - 1].SetActive(toggle);
            toggle = !toggle;
            yield return new WaitForSeconds(0.2f);
        }
        ColoredPunch[choiced[puchCode] - 1].SetActive(true);
        //4
        RedShield.SetActive(true);
        for (float size = 1f; size < 4f && keepgo; size += 0.1f)
        {
            ColoredPunch[choiced[puchCode] - 1].transform.localScale = new Vector3(size, size, size);
            yield return new WaitForSeconds(interval);
        }
        ColoredPunch[choiced[puchCode] - 1].transform.localScale = new Vector3(1, 1, 1);
        ColoredPunch[choiced[puchCode] - 1].SetActive(false);
        RedShield.SetActive(false);
        result(CheckPos(choiced[puchCode] - 1));
    }
}
