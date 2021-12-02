using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;
using System.Linq;

public class GameManage : MonoBehaviour
{

    public UIManager _UIM;
    public AnimationController _ACT;
    public Rank _RK;

    [Header("Audio")]
    public AudioSource SEAudio;
    public AudioSource BGMAudio;
    [LabeledArray(new string[] { "[0]CountSE", "[1]Start", "[2]Finish" })]
    public AudioClip[] CountDown;
    [Header("GameSetting")]
    public int RandRange;
    public GameObject[] GameScript;
    public TextMeshProUGUI titleText;
    private int StageCount = 1;
    private int RandGameNum;

    [Header("Result")]
    public GameObject resultA;
    public GameObject resultB;
    public GameObject resultC;

    public bool init_start = false;
    public bool GamePlaying;

    public int PlayCount;

    private int HeartCount = 3;
    public Image[] HeartIcon;
    [LabeledArray(new string[] { "full", "empty" })]
    public Sprite[] HeartKind;
    public int[] RandomCnt = new int[7];
    public int StageSet;

    [Header("Debug Variable")]
    public bool IgnoreFail;
    void Update()
    {
        if (init_start && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null && !GamePlaying)
        {
            HeartIcon[0].enabled = true;
            HeartIcon[1].enabled = true;
            HeartIcon[2].enabled = true;
            GamePlaying = true;
            if (PlayCount == 0)
            {
                RandomGameStart();
                PlayCount++;
            }
        }
    }
    void Start()
    {
        RandomSet();
    }

    public void RandomSet()
    {
        List<int> GachaList = new List<int>() { 0, 1, 2, 4, 5, 6, 7 };
        RandomCnt[0] = 3;
        for (int i = 1; i < 7; i++)
        {
            int rand = Random.Range(0, 7 - i);
            RandomCnt[i] = GachaList[rand];
            GachaList.RemoveAt(rand);
        }
        Debug.Log(RandomCnt[0] + " " + RandomCnt[1] + " " + RandomCnt[2] + " " + RandomCnt[3] + " " + RandomCnt[4] + " " + RandomCnt[5] + " " + RandomCnt[6]);
    }

    public void RandomGameStart()
    {
        //RandGameNum = Random.Range(0, 200);
        //RandGameNum = RandGameNum % RandRange;
        if (StageSet == -1 || StageSet == 7)
        {
            RandomSet();
            StageSet = 0;
        }
        RandGameNum = RandomCnt[StageSet];
        //--------------------Debug Code-----------------//
        //RandGameNum = 4;
        //--------------------Debug Code-----------------//
        StageSet++;
        Debug.Log("Game : " + RandGameNum);
        switch (RandGameNum)
        {
            case 0:
                FlagGame _Fg;
                _Fg = GameScript[0].GetComponent<FlagGame>();
                _Fg.Setting();
                StartCoroutine("gameStart");
                break;
            case 1:
                OXGame _OXg;
                _OXg = GameScript[1].GetComponent<OXGame>();
                _OXg.Setting();
                StartCoroutine("gameStart");
                break;
            case 2:
                AvoidPunch _AVP;
                _AVP = GameScript[2].GetComponent<AvoidPunch>();
                _AVP.Setting();
                StartCoroutine("gameStart");
                break;
            case 3:
                Squart _SQT;
                _SQT = GameScript[3].GetComponent<Squart>();
                _SQT.Setting();
                StartCoroutine("gameStart");
                break;
            case 4:
                MuGungHwa _MGW;
                _MGW = GameScript[4].GetComponent<MuGungHwa>();
                _MGW.Setting();
                StartCoroutine("gameStart");
                break;
            case 5:
                Plate _PLT;
                _PLT = GameScript[5].GetComponent<Plate>();
                _PLT.Setting();
                StartCoroutine("gameStart");
                break;
            case 6:
                PK _PK;
                _PK = GameScript[6].GetComponent<PK>();
                _PK.Setting();
                StartCoroutine("gameStart");
                break;
        }
    }

    public void GameStart()
    {
        switch (RandGameNum)
        {
            case 0:
                FlagGame _Fg;
                _Fg = GameScript[0].GetComponent<FlagGame>();
                _Fg.GameStart = true;

                break;
            case 1:
                OXGame _OXg;
                _OXg = GameScript[1].GetComponent<OXGame>();
                _OXg.GameStart = true;

                break;
            case 2:
                AvoidPunch _AVP;
                _AVP = GameScript[2].GetComponent<AvoidPunch>();
                _AVP.GameStart = true;

                break;
            case 3:
                Squart _SQT;
                _SQT = GameScript[3].GetComponent<Squart>();
                _SQT.GameStart = true;

                break;
            case 4:
                MuGungHwa _MGW;
                _MGW = GameScript[4].GetComponent<MuGungHwa>();
                _MGW.GameStart = true;

                break;
            case 5:
                Plate _PLT;
                _PLT = GameScript[5].GetComponent<Plate>();
                _PLT.GameStart = true;

                break;
            case 6:
                PK _PK;
                _PK = GameScript[6].GetComponent<PK>();
                _PK.GameStart = true;

                break;
        }
    }

    public void GameFinish(bool HeartDown)
    {
        if (HeartDown)
        {
            if (!IgnoreFail)
            {
                HeartCount -= 1;
                HeartIcon[HeartCount].sprite = HeartKind[1];
            }
            if (HeartCount == 0)
            {
                _RK.now_num = StageCount;
                _UIM.UISetOn((int)UIset.SetName.Score);
                _RK.ResultChap();
            }
            else
            {
                StageCount++;
                PlayCount--;
                GamePlaying = false;
            }
        }
        else
        {
            StageCount++;
            PlayCount--;
            GamePlaying = false;
        }
    }

    IEnumerator gameStart()
    {
        titleText.text = "Stage " + StageCount;
        SEAudio.PlayOneShot(CountDown[0]);
        yield return new WaitForSeconds(0.5f);
        SEAudio.PlayOneShot(CountDown[0]);
        yield return new WaitForSeconds(0.5f);
        SEAudio.PlayOneShot(CountDown[0]);
        yield return new WaitForSeconds(0.5f);
        SEAudio.PlayOneShot(CountDown[1]);
        GameStart();

    }

    public void ChangeTitle(float Time, string txt)
    {
        StartCoroutine(ChangeTitleTemp(Time, txt));
    }

    IEnumerator ChangeTitleTemp(float Time, string txt)
    {
        titleText.text = txt;
        yield return new WaitForSeconds(Time);
        titleText.text = "Stage " + StageCount;
    }

    public void show_Result(char result)
    {
        StartCoroutine(showResultCor(result));
    }
    IEnumerator showResultCor(char result)
    {
        float init_size = 0;
        GameObject resultPic = null;
        if (result == 'A')
        {
            resultA.SetActive(true);
            init_size = resultA.transform.localScale.x;
            resultPic = resultA;
        }
        else if (result == 'B')
        {
            resultB.SetActive(true);
            init_size = resultB.transform.localScale.x;
            resultPic = resultB;
        }
        else if (result == 'C')
        {
            resultC.SetActive(true);
            init_size = resultC.transform.localScale.x;
            resultPic = resultC;
        }
        try
        {
            resultPic.transform.localScale = new Vector3(0, 0, 0);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        float nxtSize;

        for (int i = 1; i <= 3; i++)
        {
            nxtSize = (i / 3) * init_size;
            resultPic.transform.localScale = new Vector3(nxtSize, nxtSize, nxtSize);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(1f);
        resultPic.transform.localScale = new Vector3(0, 0, 0);
        resultPic.SetActive(false);
    }
}
