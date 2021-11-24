using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateObj : MonoBehaviour
{
    public Sprite Brocken_plate;
    private Plate PlateMother;
    private Transform catchPointPos;
    bool Deadline = false;
    bool falling = true;
    bool catchPlate = false;
    float speed = 1.8f;
    void Start()
    {
        PlateMother = GameObject.FindGameObjectWithTag("plateOrigin").GetComponent<Plate>();
        catchPointPos = PlateMother.CatchPoint.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y < 248 && !Deadline)
        {
            Deadline = true;
            if (PlateMother.plateCheck(gameObject))
            {// 잡았다!
                catchPlate = true;
                gameObject.transform.parent = catchPointPos.transform;
                gameObject.transform.position = catchPointPos.transform.position;
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (25 * PlateMother.catched_plate), gameObject.transform.position.z);
            }
        }
        if (Deadline && gameObject.transform.position.y <= 0 && falling)
        {
            falling = false;
            gameObject.GetComponent<Image>().sprite = Brocken_plate;
            PlateMother.GameSound.PlayOneShot(PlateMother.SESound[1]);
        }
        else if (falling && !catchPlate)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - (speed), gameObject.transform.position.z);
        }
        if (!falling)
        {
            StartCoroutine("Destoryer");
        }
    }

    IEnumerator Destoryer()
    {
        yield return new WaitForSeconds(2f);
        PlateMother.ShowResult(false);
        Destroy(gameObject);
    }
}
