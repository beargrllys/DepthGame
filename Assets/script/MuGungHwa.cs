using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;

public class MuGungHwa : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("UI Set")]
    public Image SulLe;
    [LabeledArray(new string[] { "Hide", "Back", "Catched" })]
    public Sprite[] SulLe_spr;
    public GameObject GazyBar;
    public GameObject Gazy;
    public Image Effect;
    [LabeledArray(new string[] { "Run1", "Run2", "Stop1", "Stop2", "Die" })]
    public Sprite[] Effect_spr;
    public Image Order;
    [LabeledArray(new string[] { "Run", "Stop" })]
    public Sprite[] Order_spr;
    public Image Result;
    [LabeledArray(new string[] { "Sucess", "Fail" })]
    public Sprite[] Result_spr;
    [Header("Audio")]
    public AudioSource GameSound;
    public AudioClip[] SulLeSound;

    private bool Catched = false;
    private bool Checking = false;
    private bool Running = false;
    private Vector3 init_bodyPos;
    private float standard = 16f;
    private float Gazy_standard = 0.001f;

    public bool GameStart;
    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.MuGungHwa);
        SulLe.enabled = true;
        SulLe.sprite = SulLe_spr[0];
        Gazy.transform.localScale = new Vector3(0, 1, 1);
        Effect.enabled = true;
        Effect.sprite = Effect_spr[0];
        Result.enabled = false;
        Catched = false;
        Checking = false;
        Running = false;
        init_bodyPos = Vector3.zero;
        _ACT.Reset();
        _ACT.activeJoint[0] = true;
        _ACT.activeJoint[1] = true;
        _ACT.activeJoint[2] = true;
        _ACT.activeJoint[3] = true;
        _ACT.activeJoint[4] = true;
        _ACT.activeJoint[5] = true;
        _ACT.activeJoint[6] = true;
        _ACT.activeJoint[7] = true;
        _ACT.activeJoint[8] = true;
        _ACT.activeJoint[9] = true;
        _ACT.activeJoint[10] = true;
        _ACT.activeJoint[11] = true;
        _ACT.UpdateJoint();
    }

    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            StartCoroutine("GameRoutine");
        }
    }

    public Vector3 Col_bodyPos()
    {
        Vector3 saveVar = new Vector3(0, 0, 0);
        for (int i = 0; i < 11; i++)
        {
            saveVar += _ACT._BoneMap[i].transform.position;
        }
        return saveVar;
    }


    IEnumerator CheckingMove()
    {
        float waitStd = Random.Range(1f, 3f);
        float waitTime = 0f;
        Vector3 now_bodyPos;
        Vector3 avg_bodyPos = new Vector3(0, 0, 0); ;
        init_bodyPos = Col_bodyPos();
        int avg = 0;

        while (waitTime < waitStd)
        {
            yield return new WaitForEndOfFrame();
            waitTime += Time.deltaTime;//0번 부터 11번까지 추적
            now_bodyPos = Col_bodyPos();
            if (avg < 6)
            {
                avg_bodyPos += (init_bodyPos - now_bodyPos);
                avg++;
                continue;
            }
            else
            {
                avg = 0;
                if ((avg_bodyPos / 6).sqrMagnitude > standard)
                {
                    GameSound.PlayOneShot(SulLeSound[5]);
                    Catched = true;
                    SulLe.enabled = false;
                    Order.enabled = false;
                    Effect.sprite = Effect_spr[4];
                    Result.enabled = true;
                    Result.sprite = Result_spr[1];
                    Result.SetNativeSize();
                    Debug.Log("Fail");
                    break;
                }
                avg_bodyPos = new Vector3(0, 0, 0);
            }
        }
        yield return new WaitForSeconds(waitStd - waitTime);
        Checking = false;
        if (Catched)
        {
            _GM.GameFinish(true);
        }
    }

    IEnumerator RunningCheck(float len)
    {
        float waitTime = 0f;
        Vector3 now_bodyPos;
        Vector3 pre_bodyPos = _ACT._BoneMap[7].transform.position + _ACT._BoneMap[11].transform.position;

        while (waitTime < len)
        {
            yield return new WaitForEndOfFrame();
            waitTime += Time.deltaTime;//7번, 11번추적
            now_bodyPos = _ACT._BoneMap[7].transform.position + _ACT._BoneMap[11].transform.position;
            if ((pre_bodyPos - now_bodyPos).sqrMagnitude < 1f)
                Gazy.transform.localScale += new Vector3((pre_bodyPos - now_bodyPos).sqrMagnitude * Time.deltaTime * 1.5f, 0, 0);
            pre_bodyPos = now_bodyPos;
            if (Gazy.transform.localScale.x >= 1)
            {
                GameSound.PlayOneShot(SulLeSound[4]);
                Catched = true;
                SulLe.enabled = false;
                Order.enabled = false;
                Debug.Log("Success");
                Effect.enabled = false;
                Result.enabled = true;
                Result.sprite = Result_spr[0];
                Result.SetNativeSize();
                break;
            }

        }
        Running = false;
        if (Catched)
        {
            _GM.GameFinish(false);
        }
    }

    IEnumerator GameRoutine()
    {
        int soundSet;
        StartCoroutine(EffectChange());
        while (!Catched)
        {
            //Debug.Log("Check!");
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                SulLe.sprite = SulLe_spr[0];
                Order.sprite = Order_spr[0];
                soundSet = Random.Range(0, 3);
                GameSound.PlayOneShot(SulLeSound[soundSet]);
                Running = true;
                StartCoroutine(RunningCheck(SulLeSound[soundSet].length));
                yield return new WaitUntil(() => Running == false);
                if (!Catched)
                {
                    SulLe.sprite = SulLe_spr[1];
                    Order.sprite = Order_spr[1];
                    Checking = true;
                    StartCoroutine(CheckingMove());
                    yield return new WaitUntil(() => Checking == false);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator EffectChange()
    {
        while (!Catched)
        {
            if (Running)
            {
                Effect.sprite = Effect_spr[0];
            }
            if (Checking)
            {
                Effect.sprite = Effect_spr[2];
            }
            yield return new WaitForSeconds(0.3f);
            if (Running)
            {
                Effect.sprite = Effect_spr[1];
            }
            if (Checking)
            {
                Effect.sprite = Effect_spr[3];
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
