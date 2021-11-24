using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kinect = Windows.Kinect;

public class Tutorial : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("UI element")]
    public GameObject TutorialTotal;
    public GameObject MaleChar;
    public TextMeshProUGUI MaleText;
    public GameObject FemaleChar;
    public TextMeshProUGUI FemaleText;
    public GameObject TutoExplain;
    public GameObject OkaySign;
    public TextMeshProUGUI OkayTxt;
    public GameObject GazyBar;
    public GameObject Gazy;
    public GameObject HeartIcon;
    public Image CountDown;
    public Sprite[] CountNum = new Sprite[3];

    private string[,] script = new string[,]{
        {"안녕! 게임하러왔구나", "와..와꾸나"},
        {"이 게임은 네 동작을 \n인식해서 진행돼", "우..움직여"},
        {"게임은 총 6개고 랜덤한 \n순서로 나올거야", "모..몰라"},
        {"자세한 설명은 이걸 읽어봐", "일..거.."},
        {"주어진 하트 3개를 \n모두 소비하면 게임패배야", "딱 세번.."},
        {"그럼 잘해봐!", "빠..빠이"},
    };

    private bool TutoStep = false;

    int TotalSeq = 0;
    public void Setting()
    {
        TutoStep = true;
        StartCoroutine("ButtonUX");
        _GM.titleText.text = "튜토리얼";
        cond_talk(0);
    }

    public void cond_talk(int seg)
    {
        switch (seg)
        {
            case 0:
                StartCoroutine(CharcTalk(4f, script[0, 0], script[0, 1]));
                break;
            case 1:
                StartCoroutine(CharcTalk(4f, script[1, 0], script[1, 1]));
                break;
            case 2:
                StartCoroutine(CharcTalk(4f, script[2, 0], script[2, 1]));
                break;
            case 3:
                StartCoroutine(CharcTalk(4f, script[3, 0], script[3, 1]));
                break;
            case 4:
                TutoExplain.SetActive(true);
                MaleChar.SetActive(false);
                FemaleChar.SetActive(false);
                break;
            case 5:
                TutoExplain.SetActive(false);
                HeartIcon.SetActive(true);
                MaleChar.SetActive(true);
                FemaleChar.SetActive(true);
                StartCoroutine(CharcTalk(4f, script[4, 0], script[4, 1]));
                break;
            case 6:
                HeartIcon.SetActive(false);
                StartCoroutine(CharcTalk(4f, script[5, 0], script[5, 1]));
                break;
            case 7:
                TutoStep = false;
                StartCoroutine("CountAndStart");
                break;

        }
    }

    IEnumerator CountAndStart()
    {
        CountDown.enabled = true;
        for (int i = 0; i < 3; i++)
        {
            CountDown.sprite = CountNum[i];
            yield return new WaitForSeconds(1f);
        }
        _UIM.UISetOff((int)UIset.SetName.Tutorial);
        _GM.init_start = true;
    }
    IEnumerator CharcTalk(float time, string femaleT, string maleT)
    {
        FemaleText.text = "";
        MaleText.text = "";
        Debug.Log("Check");
        for (int i = 0; i < femaleT.Length; i++)
        {
            FemaleText.text += femaleT[i];
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(((time / 2) - (float)(femaleT.Length * 0.2) < 0) ? 0f : (float)((time / 2) - (femaleT.Length * 0.2)) / 2);
        for (int i = 0; i < maleT.Length; i++)
        {
            MaleText.text += maleT[i];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ButtonUX()
    {
        bool condR = false;
        bool condL = false;
        float WaitingT = 0f;
        float std = 2.0f;
        while (TutoStep)
        {
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                condR = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipRight, OkaySign);
                condL = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipLeft, OkaySign);

                if (condR || condL)
                {
                    float G_scale;
                    GazyBar.SetActive(true);
                    WaitingT += Time.deltaTime;

                    if (WaitingT < std)
                    {
                        G_scale = WaitingT / std;
                        Gazy.transform.localScale = new Vector3(G_scale, 1, 1);
                    }
                    else
                    {
                        WaitingT = 0f;
                        Gazy.transform.localScale = new Vector3(0, 1, 1);
                        TotalSeq++;
                        cond_talk(TotalSeq);
                    }

                }
                else
                {
                    WaitingT = 0f;
                    Gazy.transform.localScale = new Vector3(0, 1, 1);
                    GazyBar.SetActive(false);
                }

            }
        }

    }
}
