using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;

public class FlagGame : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;

    public int hardness = 0;

    /*UI Set*/
    [Header("UI Set")]
    public GameObject WhiteFlag;
    public GameObject BlueFlag;

    [Header("Audio Set")]
    public AudioSource GameSound;
    public AudioClip[] Easy;
    public int[] Easy_ans;
    public AudioClip[] Normal;
    public int[] Normal_ans;
    public AudioClip[] Hard;
    public int[] Hard_ans;

    [Header("Score")]
    private int inGameScore = 0;
    private char[] scoreStr = { '-', '-', '-', '\0' };

    public bool GameStart = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.FlagGame);
        scoreStr[0] = '-';
        scoreStr[1] = '-';
        scoreStr[2] = '-';
        inGameScore = 0;
        hardness = 0;
        _ACT.Reset();
        _ACT.activeJoint[7] = true;
        _ACT.activeJoint[11] = true;
        _ACT.UpdateJoint();
    }

    public int FlagCheck()
    {
        int result = 0; // 십의자리 백기 일의자리 청기  0:가만히 1: 내려 2: 올려
        float BlueFlagY;
        float WhiteFlagY;
        BlueFlagY = BlueFlag.transform.position.y;
        WhiteFlagY = WhiteFlag.transform.position.y;
        if (0 <= BlueFlagY && BlueFlagY < 360)
        {
            result += 1;
        }
        else if (720 <= BlueFlagY && BlueFlagY <= 1080)
        {
            result += 2;
        }
        if (0 <= WhiteFlagY && WhiteFlagY < 360)
        {
            result += 10;
        }
        else if (720 <= WhiteFlagY && WhiteFlagY <= 1080)
        {
            result += 20;
        }

        return result;
    }

    public void next_level()
    {
        if (hardness == 1)
        {
            StartCoroutine(FlagGameManager());
        }
        else if (hardness == 2)
        {
            StartCoroutine(FlagGameManager());
        }
    }

    void Update()
    {
        if (GameStart && _ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
        {
            GameStart = false;
            StartCoroutine(FlagGameManager());
            StartCoroutine("FlagWithHand");
        }
    }

    IEnumerator FlagWithHand()
    {
        Vector3 RightHandScreenPos;
        Vector3 LeftHandScreenPos;
        float BlueFlagX;
        float WhiteFlagX;
        BlueFlagX = BlueFlag.transform.position.x;
        WhiteFlagX = WhiteFlag.transform.position.x;
        while (!GameStart)
        {
            //Debug.Log(FlagCheck());
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] == null)
                break;
            RightHandScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.HandTipRight].transform.position);
            LeftHandScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.HandTipLeft].transform.position);
            BlueFlag.transform.position = new Vector3(BlueFlagX, RightHandScreenPos.y, RightHandScreenPos.z);
            WhiteFlag.transform.position = new Vector3(WhiteFlagX, LeftHandScreenPos.y, LeftHandScreenPos.z);
        }
    }

    IEnumerator FlagGameManager()
    {
        int randval = 0;
        //Debug.Log(hardness);
        if (hardness == 0)
        {
            randval = Random.Range(0, Easy.Length);
            GameSound.PlayOneShot(Easy[randval]);
            yield return new WaitForSeconds(Easy[randval].length + 0.5f);
        }
        else if (hardness == 1)
        {
            randval = Random.Range(0, Normal.Length);
            GameSound.PlayOneShot(Normal[randval]);
            yield return new WaitForSeconds(Normal[randval].length + 0.5f);
        }
        else if (hardness == 2)
        {
            randval = Random.Range(0, Hard.Length);
            GameSound.PlayOneShot(Hard[randval]);
            yield return new WaitForSeconds(Hard[randval].length + 1.0f);
        }

        if (hardness == 0)
        {
            if (Easy_ans[randval] == FlagCheck())
            {
                inGameScore += 10;
                scoreStr[0] = 'O';
            }
            else
            {
                scoreStr[0] = 'X';
            }
            _GM.ChangeTitle(1.0f, new string(scoreStr));
            yield return new WaitForSeconds(1.0f);
            hardness++;
            next_level();
        }
        else if (hardness == 1)
        {
            if (Normal_ans[randval] == FlagCheck())
            {
                inGameScore += 20;
                scoreStr[1] = 'O';
            }
            else
            {
                scoreStr[1] = 'X';
            }
            _GM.ChangeTitle(1.0f, new string(scoreStr));
            yield return new WaitForSeconds(1.0f);
            hardness++;
            next_level();
        }
        else if (hardness == 2)
        {
            if (Hard_ans[randval] == FlagCheck())
            {
                inGameScore += 30;
                scoreStr[2] = 'O';
            }
            else
            {
                scoreStr[2] = 'X';
            }
            _GM.ChangeTitle(1.0f, new string(scoreStr));
            if (inGameScore == 60)
            {
                _GM.show_Result('A');
            }
            else if (inGameScore >= 30)
            {
                _GM.show_Result('B');
            }
            else if (inGameScore < 30)
            {
                _GM.show_Result('C');
            }
            yield return new WaitForSeconds(2f);
            _GM.GameFinish();
        }
    }
}
