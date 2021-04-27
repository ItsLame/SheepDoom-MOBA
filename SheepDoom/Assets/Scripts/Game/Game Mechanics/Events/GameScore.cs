﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
namespace SheepDoom
{
    public class GameScore : NetworkBehaviour
    {
        //the display text for tower scores
        [Space(20)]
        [SerializeField]
        private Text blueCaptureCounter;

        [SerializeField]
        private Text redCaptureCounter;

        [SerializeField]
        public GameObject completeGameUI;
        public GameObject completeGameUI2;

        //counters for tower captures per team
        //hard coded for now
        [Space(20)]
        [SerializeField]
        //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float blueCaptureScore;
        [SyncVar] private float blueCaptureScore;
        [SerializeField]
        //[SyncVar(hook = nameof(updateScoreDisplayClient))] private float redCaptureScore;
        [SyncVar] private float redCaptureScore;

        public Text blueDisplay;
        public Text redDisplay;
        public Text blueScoreText;
        public Text redScoreText;
        public GameObject[] players;
        public Text BluePlayerNameText1;
        public Text BluePlayerNameText2;
        public Text BluePlayerNameText3;
        public Text RedPlayerNameText1;
        public Text RedPlayerNameText2;
        public Text RedPlayerNameText3;

        public Image BluePlayerImage1UI;
        public Image BluePlayerImage2UI;
        public Image BluePlayerImage3UI;
        public Image RedPlayerImage1UI;
        public Image RedPlayerImage2UI;
        public Image RedPlayerImage3UI;
        public string TopPlayer;
        public int TempMostKills;
        public Image BP1StarImageUI;
        public Image BP2StarImageUI;
        public Image BP3StarImageUI;
        public Image RP1StarImageUI;
        public Image RP2StarImageUI;
        public Image RP3StarImageUI;
        // Start is called before the first frame update
        /*void Start()
        {
            // changed to initialize on server only, if u put on start method, everytime new player joins, it will reinitialize to 1-1
            //initialize score
            //blueCaptureScore = 1;
            //redCaptureScore = 1;

            // they are already text components...
            //get the attached score counters text component
            //blueCaptureCounter = blueCaptureCounter.GetComponent<Text>();
            //redCaptureCounter = redCaptureCounter.GetComponent<Text>();

            //display score
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }*/

        //update score display on all clients
        public void updateScoreDisplay()
        {
            Debug.Log("blue capture score: " + blueCaptureScore);
            Debug.Log("red capture score: " + redCaptureScore);
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }

        /*
        public void updateScoreDisplayClient(float oldValue, float newValue)
        {
            blueCaptureCounter.text = blueCaptureScore.ToString();
            redCaptureCounter.text = redCaptureScore.ToString();
        }*/

        //scoring functions
        /*public void blueScoreUp()
        {
            Debug.Log("Blue score +1");
            //if blue scores, red will -1
            blueCaptureScore += 1;
            redCaptureScore -= 1;
            //update display
            updateScoreDisplay();
        }

        public void redScoreUp()
        {
            Debug.Log("Red score +1");
            //if blue scores, red will -1
            blueCaptureScore -= 1;
            redCaptureScore += 1;
            //update display
            updateScoreDisplay();
        }*/

        // causes a syncvar delay cuz this is only run on server
        public void ScoreUp(bool _byBlue, bool _byRed)
        {
            if (_byBlue && !_byRed)
            {
                blueCaptureScore += 1;
                redCaptureScore -= 1;
            }
            else if (_byRed && !_byBlue)
            {
                redCaptureScore += 1;
                blueCaptureScore -= 1;
            }
            updateScoreDisplay();
            RpcUpdateClientScoreDisplay();
        }

        // deals with syncvar delay
        [ClientRpc]
        void RpcUpdateClientScoreDisplay()
        {
            StartCoroutine(WaitForUpdate(blueCaptureScore, redCaptureScore));
        }

        private IEnumerator WaitForUpdate(float _oldBlueScore, float _oldRedScore)
        {
            while (blueCaptureScore == _oldBlueScore && redCaptureScore == _oldRedScore)
                yield return null;
            updateScoreDisplay();
        }

        // game winning condition (will be called when base is taken)
        // shows scoreboard etc when game ends, will add timer counter condition in the future
        public void GameEnd(int TeamID)
        {
            //Scoreboard display Victory/Defeat for blue team
            GameObject blueTeam = FindMe.instance.P_BlueWinLose;
            blueDisplay = blueTeam.GetComponent<Text>();

            //Scoreboard display Victory/Defeat for red team
            GameObject redTeam = FindMe.instance.P_RedWinLose;
            redDisplay = redTeam.GetComponent<Text>();

            //Scoreboard Blue Team's Score
            GameObject blueScore = FindMe.instance.P_ScoreboardBlue;
            blueScoreText = blueScore.GetComponent<Text>();
            blueScoreText.text = blueCaptureScore.ToString();

            //Scoreboard Red Team's Score
            GameObject redScore = FindMe.instance.P_ScoreboardRed;
            redScoreText = redScore.GetComponent<Text>();
            redScoreText.text = redCaptureScore.ToString();

            //Blue Team Pull UI text
            GameObject BluePlayerName1 = FindMe.instance.P_BluePlayerName1;
            BluePlayerNameText1 = BluePlayerName1.GetComponent<Text>();

            GameObject BluePlayerName2 = FindMe.instance.P_BluePlayerName2;
            BluePlayerNameText2 = BluePlayerName2.GetComponent<Text>();

            GameObject BluePlayerName3 = FindMe.instance.P_BluePlayerName2;
            BluePlayerNameText3 = BluePlayerName3.GetComponent<Text>();

            //Blue Team Pull UI image
            GameObject BluePlayerImage1 = FindMe.instance.P_BluePlayerImage1;
            BluePlayerImage1UI = BluePlayerImage1.GetComponent<Image>();

            GameObject BluePlayerImage2 = FindMe.instance.P_BluePlayerImage2;
            BluePlayerImage2UI = BluePlayerImage2.GetComponent<Image>();

            GameObject BluePlayerImage3 = FindMe.instance.P_BluePlayerImage3;
            BluePlayerImage3UI = BluePlayerImage3.GetComponent<Image>();

            //Red Team Pull UI text
            GameObject RedPlayerName1 = FindMe.instance.P_RedPlayerName1;
            RedPlayerNameText1 = RedPlayerName1.GetComponent<Text>();

            GameObject RedPlayerName2 = FindMe.instance.P_RedPlayerName2;
            RedPlayerNameText2 = RedPlayerName2.GetComponent<Text>();

            GameObject RedPlayerName3 = FindMe.instance.P_RedPlayerName3;
            RedPlayerNameText3 = RedPlayerName3.GetComponent<Text>();

            //Red Team Pull UI image
            GameObject RedPlayerImage1 = FindMe.instance.P_RedPlayerImage1;
            RedPlayerImage1UI = RedPlayerImage1.GetComponent<Image>();

            GameObject RedPlayerImage2 = FindMe.instance.P_RedPlayerImage2;
            RedPlayerImage2UI = RedPlayerImage2.GetComponent<Image>();

            GameObject RedPlayerImage3 = FindMe.instance.P_RedPlayerImage3;
            RedPlayerImage3UI = RedPlayerImage3.GetComponent<Image>();

            //Blue Team Star Player
            GameObject BP1StarImage = FindMe.instance.P_BP1Star;
            BP1StarImageUI = BP1StarImage.GetComponent<Image>();

            GameObject BP2StarImage = FindMe.instance.P_BP2Star;
            BP2StarImageUI = BP2StarImage.GetComponent<Image>();

            GameObject BP3StarImage = FindMe.instance.P_BP3Star;
            BP3StarImageUI = BP3StarImage.GetComponent<Image>();

            //Red Team Star Player
            GameObject RP1StarImage = FindMe.instance.P_RP1Star;
            RP1StarImageUI = RP1StarImage.GetComponent<Image>();

            GameObject RP2StarImage = FindMe.instance.P_RP2Star;
            RP2StarImageUI = RP2StarImage.GetComponent<Image>();

            GameObject RP3StarImage = FindMe.instance.P_RP3Star;
            RP3StarImageUI = RP3StarImage.GetComponent<Image>();

            //Character Image
            Sprite CharacterImage = Resources.Load<Sprite>("circleface");
            Debug.Log("Scoreboard: CharacterImage: " + CharacterImage);

            //Get all players' name and team id
            if (players.Length == 0)
            {
                Debug.Log("Scoreboard: FindGameObjectsWithTag");
                players = GameObject.FindGameObjectsWithTag("Player");
            }

            foreach (GameObject player in players)
            {
                Debug.Log("Scoreboard: players.Length:" + players.Length);
                for (int i = 0; i < players.Length; i++)
                {
                    //Get current player name and team
                    string name = player.GetComponent<PlayerObj>().GetPlayerName();
                    int team = (int)player.GetComponent<PlayerAdmin>().getTeamIndex();
                    int kills = (int)player.GetComponent<PlayerAdmin>().PlayerKills;
                    if (TempMostKills < kills)
                    {
                        TempMostKills = kills;
                        TopPlayer = name;
                    }

                    if (team == 1) //If current player is Blue Team
                    {
                        //Put name & image into UI 
                        if (BluePlayerNameText1.text != null)
                        {
                            BluePlayerNameText1.text = name;
                            BluePlayerImage1UI.sprite = CharacterImage;
                            BluePlayerImage1UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: BluePlayerNameText1: " + name);
                        }
                        else if (BluePlayerNameText2.text != null)
                        {
                            BluePlayerNameText2.text = name;
                            BluePlayerImage2UI.sprite = CharacterImage;
                            BluePlayerImage2UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: BluePlayerNameText2: " + name);
                        }
                        else if (BluePlayerNameText3.text != null)
                        {
                            BluePlayerNameText3.text = name;
                            BluePlayerImage3UI.sprite = CharacterImage;
                            BluePlayerImage3UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: BluePlayerNameText3: " + name);
                        }
                    }
                    else if (team == 2) //If current player is Red Team
                    {
                        //Put name & image into UI 
                        if (RedPlayerNameText1.text != null)
                        {
                            RedPlayerNameText1.text = name;
                            RedPlayerImage1UI.sprite = CharacterImage;
                            RedPlayerImage1UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: RedPlayerNameText1: " + name);
                        }
                        else if (RedPlayerNameText2.text != null)
                        {
                            RedPlayerNameText2.text = name;
                            RedPlayerImage2UI.sprite = CharacterImage;
                            RedPlayerImage2UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: RedPlayerNameText2: " + name);
                        }
                        else if (RedPlayerNameText3.text != null)
                        {
                            RedPlayerNameText3.text = name;
                            RedPlayerImage3UI.sprite = CharacterImage;
                            RedPlayerImage3UI.color = new Color32(255, 255, 255, 255);
                            Debug.Log("Scoreboard: RedPlayerNameText3: " + name);
                        }
                    }
                }
            }
            //Make star player image show
            if (BluePlayerNameText1.text == TopPlayer)
            {
                BP1StarImageUI.color = new Color32(255, 255, 255, 255);
            }
            else if (BluePlayerNameText2.text == TopPlayer)
            {
                BP2StarImageUI.color = new Color32(255, 255, 255, 255);
            }
            else if (BluePlayerNameText3.text == TopPlayer)
            {
                BP3StarImageUI.color = new Color32(255, 255, 255, 255);
            }
            else if (RedPlayerNameText1.text == TopPlayer)
            {
                RP1StarImageUI.color = new Color32(255, 255, 255, 255);
            }
            else if (RedPlayerNameText2.text == TopPlayer)
            {
                RP2StarImageUI.color = new Color32(255, 255, 255, 255);
            }
            else if (RedPlayerNameText3.text == TopPlayer)
            {
                RP3StarImageUI.color = new Color32(255, 255, 255, 255);
            }
           

            //if blue team wins
            if (TeamID == 1)
            {
                Debug.Log("Blue Team Wins!");
                blueDisplay.text = "Victory";
                redDisplay.text = "Defeat";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }

            //if red team wins, not gonna use else for precision
            if (TeamID == 2)
            {
                Debug.Log("Red Team Wins!");
                blueDisplay.text = "Defeat";
                redDisplay.text = "Victory";
                completeGameUI.SetActive(true);
                completeGameUI.GetComponent<Animator>().SetTrigger("Complete");
            }
        }

        public override void OnStartServer()
        {
            blueCaptureScore = 1;
            redCaptureScore = 1;
            updateScoreDisplay();
        }

        public override void OnStartClient()
        {
            updateScoreDisplay(); // when start on client, it will automatically take values from server
        }
    }
}