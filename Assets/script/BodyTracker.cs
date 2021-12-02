using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTracker : MonoBehaviour // UI와 Joint 겹침여부 확인/ 기능적인 코드 포함
{
    public Camera mainCamera;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool jointTo2D_judge(Vector3 jointPos, Vector2 UIPos, Vector2 size, Vector3 scale)
    {
        float x_min, x_max;
        float y_min, y_max;
        x_min = UIPos.x - (size.x * scale.x * 0.5f);
        x_max = UIPos.x + (size.x * scale.x * 0.5f);
        y_min = UIPos.y - (size.y * scale.y * 0.5f);
        y_max = UIPos.y + (size.y * scale.y * 0.5f);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(jointPos);
        screenPos.z = 0;

        if (screenPos.x < x_max && (screenPos.x > x_min))
        {
            if (screenPos.y < y_max && (screenPos.y > y_min))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    public Vector3 jointTo2D_pos(Vector3 jointPos)
    {

        Vector3 screenPos = mainCamera.WorldToScreenPoint(jointPos);
        screenPos.z = 0;

        return screenPos;
    }
}
