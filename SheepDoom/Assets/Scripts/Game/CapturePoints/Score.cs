﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    //the display text for tower scores
    [Space(20)]
    [SerializeField]
    private Text blueCaptureCounter;

    [SerializeField]
    private Text redCaptureCounter;


    //counters for tower captures per team
    //hard coded for now
    [Space(20)]
    [SerializeField]
    private float blueCaptureScore = 0;
    [SerializeField]
    private float redCaptureScore = 2;

    // Start is called before the first frame update
    void Start()
    {
        //get the attached score counters text component
        blueCaptureCounter = blueCaptureCounter.GetComponent<Text>();
        redCaptureCounter = redCaptureCounter.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //display both of em on screen
        blueCaptureCounter.text = "Blue:" + blueCaptureScore;
        redCaptureCounter.text = "Red:" + redCaptureScore;
    }

    //scoring functions
    //its bad to make this public right? <----------------------------------------------- help
    public void blueScoreUp()
    {
        //if blue scores, red will -1
        blueCaptureScore += 1;
        redCaptureScore -= 1;
    }
}
