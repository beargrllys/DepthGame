using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Kinect = Windows.Kinect;
using Firebase.Database;
using Firebase;
public class Rank : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("Ranking")]
    public string DBurl = "https://mobileappdb-2fefb.firebaseio.com/";
    //public Text test;
    public string txt = "";
    public int DBCount = 0;
    public Image[] first = new Image[5];
    public Image[] second = new Image[5];
    public Image[] thrid = new Image[5];
    public Image[] forth = new Image[5];
    public Image[] fifth = new Image[5];
    public Sprite[] alpha = new Sprite[26];
    public Sprite[] number = new Sprite[10];

    private List<ScoreSet> ReadedData;
    private bool settingDone = false;

    DatabaseReference reference;

    [Header("RankInput")]
    public string now_name = "AAA";
    public int now_num;
    public Image[] nameSpell = new Image[3];
    public Image[] numSpell = new Image[2];
    public GameObject[] upBtn = new GameObject[3];
    public GameObject[] downBtn = new GameObject[3];
    public GameObject GazyBar;
    public GameObject Gazy;
    public GameObject OkaySign;
    public GameObject ExGazyBar;
    public GameObject ExGazy;
    public GameObject ExOkaySign;
    public AudioSource BGM;
    private Image[] _upBtn = new Image[3];
    private Image[] _downBtn = new Image[3];
    private bool TrackingMode = true;
    private bool goRankBtn = true;
    private bool goExitBtn = true;
    void Start()
    {
        AppOptions options = new AppOptions { DatabaseUrl = new Uri(DBurl) };
        FirebaseApp app = FirebaseApp.Create(options);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        for (int i = 0; i < 3; i++)
        {
            _upBtn[i] = upBtn[i].GetComponent<Image>();
            _downBtn[i] = downBtn[i].GetComponent<Image>();
        }
        //ResultChap();
    }
    public void ResultChap()
    {
        StartCoroutine("DebugStart");
    }
    IEnumerator DebugStart()
    {
        while (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] == null)
        {
            yield return new WaitForEndOfFrame();
        }
        _UIM.UISetOn((int)UIset.SetName.Score);
        BGM.Play();
        _ACT.Reset();
        _ACT.activeJoint[11] = true;
        _ACT.UpdateJoint();
        StartCoroutine("HandTracking");
        StartCoroutine("ButtonUX");
    }
    IEnumerator HandTracking()
    {
        int feedNum = -1;
        int prefeedNum = -1;
        numSpell[0].sprite = number[now_num / 10];
        numSpell[1].sprite = number[now_num % 10];
        while (TrackingMode)
        {
            yield return new WaitForEndOfFrame();
            char[] txt = now_name.ToCharArray();
            if (_ACT.judgeByGameObject(11, upBtn[0]))
                feedNum = 0;
            else if (_ACT.judgeByGameObject(11, upBtn[1]))
                feedNum = 1;
            else if (_ACT.judgeByGameObject(11, upBtn[2]))
                feedNum = 2;
            else if (_ACT.judgeByGameObject(11, downBtn[0]))
                feedNum = 3;
            else if (_ACT.judgeByGameObject(11, downBtn[1]))
                feedNum = 4;
            else if (_ACT.judgeByGameObject(11, downBtn[2]))
                feedNum = 5;
            else
                feedNum = -1;
            if (feedNum != -1 && feedNum != prefeedNum)
            {
                switch (feedNum)
                {
                    case 0:
                        increaseLetter(nameSpell[0], txt[0] - 'A', 0);
                        break;
                    case 1:
                        increaseLetter(nameSpell[1], txt[1] - 'A', 1);
                        break;
                    case 2:
                        increaseLetter(nameSpell[2], txt[2] - 'A', 2);
                        break;
                    case 3:
                        decreaseLetter(nameSpell[0], txt[0] - 'A', 0);
                        break;
                    case 4:
                        decreaseLetter(nameSpell[1], txt[1] - 'A', 1);
                        break;
                    case 5:
                        //Debug.Log(_downBtn.Length + " / " + txt.Length + " / " + 2);
                        decreaseLetter(nameSpell[2], txt[2] - 'A', 2);
                        break;
                }
            }
            prefeedNum = feedNum;
            feedNum = -1;
        }
    }

    IEnumerator ButtonUX()
    {
        bool condR = false;
        float WaitingT = 0f;
        float std = 2.0f;
        while (goRankBtn)
        {
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                condR = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipRight, OkaySign);
                //condL = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipLeft, OkaySign);

                if (condR)
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
                        WriteDB(now_name, now_num);
                        StartCoroutine(DBSetting());
                        StartCoroutine(ExitButtonUX());
                        _UIM.UISetOff((int)UIset.SetName.Score);
                        _UIM.UISetOn((int)UIset.SetName.Rank);
                        goRankBtn = false;
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

    IEnumerator ExitButtonUX()
    {
        bool condR = false;
        float WaitingT = 0f;
        float std = 2.0f;
        while (goExitBtn)
        {
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null)
            {
                condR = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipRight, ExOkaySign);
                //condL = _ACT.judgeByGameObject((int)Kinect.JointType.HandTipLeft, OkaySign);

                if (condR)
                {
                    float G_scale;
                    ExGazyBar.SetActive(true);
                    WaitingT += Time.deltaTime;

                    if (WaitingT < std)
                    {
                        G_scale = WaitingT / std;
                        ExGazy.transform.localScale = new Vector3(G_scale, 1, 1);
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false; //play모드를 false로.
#else
                        Application.Quit(); //어플리케이션 종료
#endif
                    }

                }
                else
                {
                    WaitingT = 0f;
                    ExGazy.transform.localScale = new Vector3(0, 1, 1);
                    ExGazyBar.SetActive(false);
                }
            }
        }
    }

    public void increaseLetter(Image image, int nowLetter, int index)
    {
        Debug.Log(nowLetter);
        image.sprite = alpha[(nowLetter + 1) % 26];
        char[] txt = { now_name[0], now_name[1], now_name[2] };
        txt[index] = Convert.ToChar(((nowLetter + 1) % 26) + 'A');
        now_name = txt[0].ToString() + txt[1].ToString() + txt[2].ToString();
    }
    public void decreaseLetter(Image image, int nowLetter, int index)
    {
        Debug.Log(nowLetter);
        if (nowLetter == 0)
            nowLetter = 26;
        int k = (nowLetter - 1) % 26;
        image.sprite = alpha[k];
        char[] txt = { now_name[0], now_name[1], now_name[2] };
        txt[index] = Convert.ToChar(k + 'A');
        now_name = txt[0].ToString() + txt[1].ToString() + txt[2].ToString();
    }
    public void setAllData()
    {
        List<ScoreSet> DisplayData = new List<ScoreSet>();
        int maxscore = -1;
        int maxidx = -1;
        for (int i = 0; i < 5; i++)
        {
            if (i == ReadedData.Count)
            {
                break;
            }
            for (int k = 0; k < ReadedData.Count; k++)
            {
                if (maxscore < ReadedData[k].score)
                {
                    maxscore = ReadedData[k].score;
                    maxidx = k;
                }
            }
            Debug.Log("Check! " + maxscore + " / " + maxidx);
            if (maxscore != -1)
            {
                DisplayData.Add(new ScoreSet(ReadedData[maxidx].name, ReadedData[maxidx].score));
                ReadedData[maxidx].score = -1;
                Debug.Log("Check!");
                maxscore = -1;
                maxidx = -1;
            }

        }
        Image[][] NumList = { first, second, thrid, forth, fifth };
        for (int i = 0; i < NumList.Length; i++)
        {
            Debug.Log(DisplayData.Count);
            if (i < DisplayData.Count)
            {
                string name = DisplayData[i].name;
                int charIdx = name[0] - 'A';
                NumList[i][0].sprite = alpha[charIdx];
                charIdx = name[1] - 'A';
                NumList[i][1].sprite = alpha[charIdx];
                charIdx = name[2] - 'A';
                NumList[i][2].sprite = alpha[charIdx];
                NumList[i][3].sprite = number[DisplayData[i].score / 10];
                NumList[i][4].sprite = number[DisplayData[i].score % 10];
            }
            else
            {
                string name = "XXX";
                int charIdx = name[0] - 'A';
                NumList[i][0].sprite = alpha[charIdx];
                charIdx = name[1] - 'A';
                NumList[i][1].sprite = alpha[charIdx];
                charIdx = name[2] - 'A';
                NumList[i][2].sprite = alpha[charIdx];
                NumList[i][3].sprite = number[0];
                NumList[i][4].sprite = number[0];
            }
        }
    }

    public void WriteDB(string name, int stage)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        ScoreSet data = new ScoreSet(name, stage);
        string jsondata = JsonUtility.ToJson(data);
        string key = reference.Push().Key;
        reference.Child(key).SetRawJsonValueAsync(jsondata);
    }
    public void ReadDB()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        ReadCount();
        //ReadedData = new ScoreSet[DBCount];
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                int idx = 0;
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary GPSData = (IDictionary)data.Value;
                    Debug.Log(int.Parse(GPSData["score"].ToString()));
                    ScoreSet tmp = new ScoreSet(GPSData["name"].ToString(), int.Parse(GPSData["score"].ToString()));
                    //ReadedData[idx] = tmp;
                    Check(idx, tmp);
                    idx++;
                    Debug.Log("Name : " + GPSData["name"].ToString() + " / score : " + int.Parse(GPSData["score"].ToString()));
                }
                //Debug.Log(ReadedData.Length);
                //setAllData();
            }

        });
    }

    public void pushData(List<ScoreSet> list)
    {
        ReadedData = list;
        for (int i = 0; i < ReadedData.Count; i++)
        {
            Debug.Log(ReadedData[i].name);
        }
        settingDone = true;
    }

    IEnumerator DBSetting()
    {
        GetInformation<List<ScoreSet>>(pushData);
        while (!settingDone)
        {
            yield return new WaitForEndOfFrame();
        }
        setAllData();
    }

    public void GetInformation<T>(Action<List<ScoreSet>> callback) where T : new()
    {
        var list = new List<ScoreSet>();
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                var snapshot = task.Result;
                foreach (var data in snapshot.Children)
                {
                    IDictionary info = (IDictionary)data.Value;

                    list.Add(new ScoreSet(info["name"].ToString(), int.Parse(info["score"].ToString())));
                }
            }
            callback(list);
        });
    }
    public void ReadCount()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DBCount = (int)snapshot.ChildrenCount;
            }

        });
    }

    public void Check(int idx, ScoreSet tmp)
    {
        ReadedData[idx] = tmp;
        Debug.Log("Check!");
    }
}

public class ScoreSet
{
    public string name = "";
    public int score = 0;

    public ScoreSet(string Name, int Score)
    {
        name = Name;
        score = Score;
    }
}