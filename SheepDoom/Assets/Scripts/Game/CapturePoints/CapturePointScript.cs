﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CapturePointScript : MonoBehaviour
{
    //attach the score gameobject to count the score
    public GameObject scoreGameObject;

    //tower hp counters
    [Space(20)]
    [SerializeField]
    //base hp
    [Tooltip("How much HP the tower has, edit this")]
    private float TowerHP;
    [SerializeField]
    private float TowerInGameHP; //to be used in game, gonna be the one fluctuating basically

    //rate of capture
    [SerializeField]
    private float TowerCaptureRate;

    //regeneration rate if not under capture
    [SerializeField]
    private float TowerRegenRate;

    //captured bools
    [Space(20)]
    [SerializeField]
    private bool CapturedByBlue;
    [SerializeField]
    private bool CapturedByRed;
    [SerializeField]
    private int numOfCapturers; //logging number to check if tower is under capture or not

    public event Action<float> OnHealthPctChangedTower = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        //set the tower's hp based on the settings
        TowerInGameHP = TowerHP;

        //single player mode, red team ownership at start
        CapturedByRed = true;

        //no one is capturing it at start so put at 0
        numOfCapturers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //regen hp if tower is not under capture
        if ((numOfCapturers == 0) && (TowerInGameHP < TowerHP))
        {
            //TowerInGameHP += TowerRegenRate * Time.deltaTime;
            modifyinghealth(TowerRegenRate * Time.deltaTime);
            //debug showing tower HP
            Debug.Log(this.name + " HP: " + TowerInGameHP);
        }

        //once HP = 0, notify the scoring and convert the tower
        //for now since single player mode, only use blue team's settings
        if (TowerInGameHP <= 0 && !CapturedByBlue)
        {
            //show which point is captured, change point authority and max out towerHP
            Debug.Log(this.name + " Captured By Blue Team");
            CapturedByBlue = true;
            CapturedByRed = false;

            modifyinghealth(TowerHP);
          //  TowerInGameHP = TowerHP;

            //reference the score script to increase score function
            scoreGameObject.GetComponent<Score>().blueScoreUp();
        }

        //change color when captured by blue
        if (CapturedByBlue)
        {
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.blue);
        }

        //else its red
        else
        {
            var captureRenderer = this.GetComponent<Renderer>();
            captureRenderer.material.SetColor("_Color", Color.red);
        }
    }

    public void modifyinghealth(float amount)
    {
        TowerInGameHP += amount;
        Debug.Log("health: tower in game hp:  " + TowerInGameHP);
        float currenthealthPct = TowerInGameHP /TowerHP;
        OnHealthPctChangedTower(currenthealthPct);
        Debug.Log("health tower ================================== changed");
    }

    //check for player enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player In Zone");
            numOfCapturers += 1;
        }
    }

    //for capture hp reduction when staying in area
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //single player mode, so only blue team
            if (!CapturedByBlue)
            {
                Debug.Log(other.name + "capturing Tower");

                modifyinghealth(-(TowerCaptureRate * Time.deltaTime));
                //TowerInGameHP -= TowerCaptureRate * Time.deltaTime;

                //debug showing tower HP
                Debug.Log(this.name + " HP: " + TowerInGameHP);
            }
        }
    }

    //check for player exit
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Left Zone");
            numOfCapturers -= 1;
        }
    }

}