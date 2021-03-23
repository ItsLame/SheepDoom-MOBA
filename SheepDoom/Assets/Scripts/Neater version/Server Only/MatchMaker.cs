﻿using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;

// This script only runs on the server, not available to clients
namespace SheepDoom
{
    [System.Serializable]
    public class Match
    {
        private string matchID;
        private int team1Count;
        private int team2Count;
        private int countReady;
        private SyncListGameObject players = new SyncListGameObject();

        public Match(string matchID, GameObject player)
        {
            this.matchID = matchID;
            players.Add(player);
            team1Count++;
            countReady++;
        }

        public string GetMatchID()
        {
            return matchID;
        }

        public SyncListGameObject GetPlayerObjList()
        {
            return players;
        }

        public int GetTeam1Count()
        {
            return team1Count;
        }

        public void SetTeam1Count(int _team1Count)
        {
            team1Count = _team1Count;
        }
        
        public void AddTeam1Count()
        {
            team1Count++;
        }

        public void MinusTeam1Count()
        {
            team1Count--;
        }

        public int GetTeam2Count()
        {
            return team2Count;
        }

        public void SetTeam2Count(int _team2Count)
        {
            team2Count = _team2Count;
        }

        public void AddTeam2Count()
        {
            team2Count++;
        }

        public void MinusTeam2Count()
        {
            team2Count--;
        }

        public Match() { }
    }

    [System.Serializable]
    // to store a list of game objects that needs to be synced between clients
    public class SyncListGameObject : SyncList<GameObject>
    {
    }

    [System.Serializable]
    // to store the list of matches of class Match
    public class SyncListMatch : SyncList<Match>
    {
    }

    public class MatchMaker : NetworkBehaviour
    {
        public static MatchMaker instance;

        // track matches
        private SyncListMatch matches = new SyncListMatch();
        private SyncList<string> matchIDs = new SyncList<string>();
        private readonly SyncDictionary<string, Scene> subLobbyScenes = new SyncDictionary<string, Scene>();
        [SerializeField] GameObject lobbyManager;
        GameObject matchLobby;
        private GameObject lobby = null;
        private int matchIndex = 0;
        private string matchID = "";

        void Start()
        {
            instance = this;
        }

        #region Get

        public SyncDictionary<string, Scene> GetLobbyScenes()
        {
            return subLobbyScenes;
        }
        public int GetMatchCount()
        {
            return matches.Count;
        }
        public int GetTeam1Count()
        {
            return matches[GetMatchIndex()].GetTeam1Count();
        }

        public int GetTeam2Count()
        {
            return matches[GetMatchIndex()].GetTeam2Count();
        }
        public int GetMatchIndex()
        {
            return matchIndex;
        }

        public string GetMatchID()
        {
            return matchID;
        }

        public SyncList<GameObject> GetPlayerObjList(int _matchIndex)
        {
            return matches[_matchIndex].GetPlayerObjList();
        }

        public GameObject GetLobby()
        {
            return lobby;
        }

        #endregion

        #region Set

        public void SetTeam1Count(int _team1Count)
        {
            matches[GetMatchIndex()].SetTeam1Count(_team1Count);
        }
        public void SetTeam2Count(int _team2Count)
        {
            matches[GetMatchIndex()].SetTeam2Count(_team2Count);
        }

        public void SetMatchIndex(int _matchIndex)
        {
            matchIndex = _matchIndex;
        }

        public void SetMatchID(string _matchID)
        {
            matchID = _matchID;
        }

        public void SetLobby(GameObject _lobby)
        {
            lobby = _lobby;
        }

        #endregion

        //generate random match ID
        public static string GetRandomMatchID()
        {
            //create empty string 
            string _id = string.Empty;
            //generate time            
            for (int i = 0; i < 5; i++)
            {
                //generate random letter / number, 0~26 = letter and 27~36 = number
                int random = UnityEngine.Random.Range(0, 36);

                //letters
                if (random < 26)
                {
                    //randomly get char, capital + non caps
                    _id += (char)(random + 65);
                }

                //numbers
                else
                {
                    _id += (random - 26).ToString();
                }
            }

            Debug.Log($"Random Match ID: {_id}");
            return _id;
        }

        public bool HostGame(string _matchID, GameObject _player)
        {
            if(!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                matches.Add(new Match(_matchID, _player));
                matchLobby = Instantiate(lobbyManager);
                NetworkServer.Spawn(matchLobby);
                matchLobby.GetComponent<LobbyManager>().SetMatchID(_matchID);
                matchLobby.GetComponent<LobbyManager>().StartLobbyScene();
                _player.GetComponent<PlayerObj>().SetTeamIndex(1); // syncvared
                _player.GetComponent<PlayerObj>().SetPlayerSortIndex(1); // syncvared

                SetMatchID(_matchID);
                SetMatchIndex(matchIDs.IndexOf(_matchID));
                SetLobby(matchLobby);

                TeamCount(GetMatchIndex(), _player);

                return true;
            }
            else
            {
                Debug.Log("Match ID already exists");
                return false;
            }
        }

        public bool JoinGame(string _matchID, GameObject _player)
        {
            if(matchIDs.Contains(_matchID))
            {
                for(int i = 0; i < matches.Count; i++)
                {
                    if(_matchID == matches[i].GetMatchID())
                    {
                        matches[i].GetPlayerObjList().Add(_player);
                        if(matches[i].GetTeam1Count() < 3)
                        {
                            _player.GetComponent<PlayerObj>().SetTeamIndex(1);
                            _player.GetComponent<PlayerObj>().SetPlayerSortIndex(matches[i].GetTeam1Count());

                            TeamCount(i, _player);

                            SetMatchID(_matchID);
                            SetMatchIndex(matchIDs.IndexOf(_matchID));
                        }
                        else if(matches[i].GetTeam2Count() < 3)
                        {
                            _player.GetComponent<PlayerObj>().SetTeamIndex(2);
                            _player.GetComponent<PlayerObj>().SetPlayerSortIndex(matches[i].GetTeam2Count());

                            TeamCount(i, _player);

                            SetMatchID(_matchID);
                            SetMatchIndex(matchIDs.IndexOf(_matchID));
                        }
                        break;
                    }
                }
                return true;
            }
            else
            {
                Debug.Log("Match ID does not exist");
                return false;
            }
        }

        public void TeamCount(int _matchIndex, GameObject _player)
        {
            if(_player.GetComponent<PlayerObj>().GetUpdateCount() == false)
            {
                if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                {
                    matches[_matchIndex].AddTeam1Count();
                   
                    GetLobby().GetComponent<LobbyManager>().myTeam1Count++;

                    Debug.Log("@MatchMaker(new player) -> lobbymanager team1count: " + GetLobby().GetComponent<LobbyManager>().myTeam1Count);
                }
                else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                {
                    matches[matchIndex].AddTeam2Count();

                    GetLobby().GetComponent<LobbyManager>().myTeam2Count++;

                    Debug.Log("@MatchMaker(new player) -> lobbymanager team2count: " + GetLobby().GetComponent<LobbyManager>().myTeam1Count);    
                }
            }
            else if(_player.GetComponent<PlayerObj>().GetUpdateCount() == true)
            {
                if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 1)
                {
                    matches[_matchIndex].AddTeam1Count();
                    matches[_matchIndex].MinusTeam2Count();

                    GetLobby().GetComponent<LobbyManager>().myTeam1Count++;
                    GetLobby().GetComponent<LobbyManager>().myTeam2Count--;

                    Debug.Log("@MatchMaker -> lobbymanager team1count: " + GetLobby().GetComponent<LobbyManager>().myTeam1Count);
                }
                else if(_player.GetComponent<PlayerObj>().GetTeamIndex() == 2)
                {
                    matches[_matchIndex].AddTeam2Count();
                    matches[_matchIndex].MinusTeam1Count();

                    GetLobby().GetComponent<LobbyManager>().myTeam2Count++;
                    GetLobby().GetComponent<LobbyManager>().myTeam1Count--;

                    Debug.Log("@MatchMaker -> lobbymanager team2count: " + GetLobby().GetComponent<LobbyManager>().myTeam2Count);
                }
            }
        }

        #region Start & Stop Callbacks

        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() { }

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient() { }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        /// <summary>
        /// Called when the local player object has been set up.
        /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
        /// </summary>
        public override void OnStartLocalPlayer() { }

        /// <summary>
        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
        /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStartAuthority() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }

    //converting id to guid using some service
    //convert 5 digit string into a guid
    //remember hashing in system security? huehehue
    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            //create instance
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            //copied word for word as its the same enconding thing
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }
    }
}
