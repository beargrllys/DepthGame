using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using TMPro;

public class Plate : MonoBehaviour
{
    public UIManager _UIM;
    public BodyTracker _BT;
    public AnimationController _ACT;
    public GameManage _GM;
    [Header("UI Set")]
    public GameObject Character;
    public GameObject CatchPoint;
    public GameObject Plate_obj;
    public GameObject Parent_obj;
    public Vector3[] SpwanPoint;
    public Image Order;
    [LabeledArray(new string[] { "CatchIt", "Sucess", "Fail" })]
    public Sprite[] Order_spr;
    [Header("Audio")]
    public AudioSource GameSound;
    [LabeledArray(new string[] { "whistle", "broken" })]
    public AudioClip[] SESound;
    public int catched_plate = 0;
    public int spwaned_plate = 5;
    private bool done = false;
    public bool GameStart;
    // Start is called before the first frame update

    void Update()
    {
        if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] != null && GameStart)
        {
            GameStart = false;
            Order.enabled = false;
            StartCoroutine("plateSpawn");
            StartCoroutine("moveCharc");
        }
    }

    public void Setting()
    {
        _UIM.UISetOn((int)UIset.SetName.Plate);
        _ACT.Reset();
        _ACT.activeJoint[2] = true;
        _ACT.UpdateJoint();
        Order.sprite = Order_spr[0];
        Order.enabled = true;
        catched_plate = 0;
        done = true;
    }

    public bool plateCheck(GameObject plate)
    {
        if (plate.transform.position.x > (CatchPoint.transform.position.x - 150f) && plate.transform.position.x < (CatchPoint.transform.position.x + 150f))
        {
            catched_plate += 1;
            if (catched_plate == spwaned_plate)
            {
                ShowResult(true);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ShowResult(bool result)
    {
        StartCoroutine(ResultShow(result));
    }
    public void GameOver()
    {
        StopAllCoroutines();
        if (catched_plate == 5)
        {
            _GM.GameFinish(false);
        }
        else
        {
            _GM.GameFinish(true);
        }
    }
    IEnumerator ResultShow(bool result)
    {
        if (result)
        {
            Order.sprite = Order_spr[1];
            GameObject[] objs = GameObject.FindGameObjectsWithTag("plateObj");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
            Order.enabled = true;
            yield return new WaitForSeconds(2f);
        }
        else
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("plateObj");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
            Order.enabled = true;
            Order.sprite = Order_spr[2];
            yield return new WaitForSeconds(2f);
        }
        GameOver();
    }

    // Update is called once per frame
    IEnumerator plateSpawn()
    {
        int spwNum = 0;
        int preSpwNum = 0;
        float termTime = 0;
        for (int i = 0; i < spwaned_plate; i++)
        {
            while (preSpwNum == spwNum)
                spwNum = Random.Range(0, 4);
            preSpwNum = spwNum;
            termTime = Random.Range(1f, 2f);
            GameObject newPlate = Instantiate(Plate_obj, SpwanPoint[spwNum], Quaternion.identity);
            newPlate.transform.parent = Parent_obj.transform;
            yield return new WaitForSeconds(termTime);
        }
    }
    IEnumerator moveCharc()
    {
        Vector3 HeadScreenPos = new Vector3(0, 0, 0);
        Debug.Log("Check!" + done);
        while (done)
        {
            yield return new WaitForEndOfFrame();
            if (_ACT._BoneMap[(int)Kinect.JointType.SpineBase] == null)
                continue;
            HeadScreenPos = _BT.jointTo2D_pos(_ACT._BoneMap[(int)Kinect.JointType.Neck].transform.position);
            Character.transform.position = new Vector3(HeadScreenPos.x, Character.transform.position.y, Character.transform.position.z);
        }
    }
}
