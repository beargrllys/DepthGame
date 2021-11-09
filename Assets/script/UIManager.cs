using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [LabeledArray(new string[] { "Intro", "FlagGame", "OXGame", "AvoidPunch", "Squart" })]
    public GameObject[] UISet;

    public void UISetOff(int setName)
    {
        //int idx = (int)setName.ToEnum<UIset.SetName>();
        UISet[setName].SetActive(false);
    }

    public void UISetOn(int setName)
    {
        for (int i = 0; i < UISet.Length; i++)
        {
            UISet[i].SetActive(false);
        }
        UISet[setName].SetActive(true);
    }
}
