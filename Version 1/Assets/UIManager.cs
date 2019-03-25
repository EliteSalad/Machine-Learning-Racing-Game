using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static float minuteCount, secondCount, millCount;
    public static string millDisplay;
    public GameObject minBox, secBox, milBox;
    public GameObject minBox2, secBox2, milBox2;
    public GameObject minBox3, secBox3, milBox3;
    public bool newLap = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LapTimeManager();
        if (newLap)
        {
            LapComplete();
            newLap = false;
        }
    }
    void LapTimeManager()
    {
        millCount += Time.deltaTime * 10;
        millDisplay = millCount.ToString("F0");
        milBox.GetComponent<Text>().text = "" + millDisplay;

        if (millCount >= 10)
        {
            millCount = 0;
            secondCount += 1;
        }
        if (secondCount <= 9)
            secBox.GetComponent<Text>().text = "0" + secondCount + ".";
        else
            secBox.GetComponent<Text>().text = "" + secondCount + ".";

        if (secondCount >= 60)
        {
            secondCount = 0;
            minuteCount += 1;
        }

        if (minuteCount <= 9)
            minBox.GetComponent<Text>().text = "0" + minuteCount + ":";
        else
            minBox.GetComponent<Text>().text = "" + minuteCount + ":";

    }

    void LapComplete()
    {
        //Make current time the last lap display
        minBox2.GetComponent<Text>().text = minBox.GetComponent<Text>().text;
        secBox2.GetComponent<Text>().text = secBox.GetComponent<Text>().text;
        milBox2.GetComponent<Text>().text = milBox.GetComponent<Text>().text;

        //reset old one
        millCount = 0;
        minuteCount = 0;
        secondCount = 0;

        //increase Lap counter
        //if best lap is 00 or minbox * secbox * milbox < (minbox*60*10) * (secbox*60) * (milbox)
        NewBestLap();
    }

    void NewBestLap()
    {

    }
    void Counntdown()
    {

    }

    void RaceFinish()
    {
        //Animate camera
        //Show score
    }

    void Reset()
    {
        //Set timer
        //r
    }
}
