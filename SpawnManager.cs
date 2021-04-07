﻿using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SheepDoom
{
    public class SpawnManager : NetworkBehaviour
    {
        [Header("UI Attack Buttons")]
        public Button _NormalButton;
        public Button _SpecialButton;
        public Button _UltiButton;

        [Space(15)]
        public static SpawnManager instance;
        
        [Header("Setting up player")]
        //[SerializeField]
        //private NetworkIdentity playerPrefab = null;
        [SerializeField]
        private NetworkIdentity gameplayPlayerPrefab = null;
        private GameObject currentPlayerObj = null;
        private ClientName _cn;

        // dynamically store and call functions and dispatched on the player object spawned by the client
        // note that client prefab/object and player prefab/object are 2 different things but are connected
        public static event Action<GameObject> OnClientPlayerSpawned;

        void Awake()
        {
            _cn = GetComponent<ClientName>();
        }

        // This function will be called when player object is spawned for a client, make sure to pass the player obj
        // Once this function is called, it will retrieve the relevant function within OnClientPlayerSpawned Actions and call it for the player obj
        public void InvokePlayerObjectSpawned(GameObject _player)
        {
            currentPlayerObj = _player;
            _cn.SetClientName();
            _cn.SetPlayerName(_cn.GetClientName());
            Debug.Log("Player object spawned");
            OnClientPlayerSpawned?.Invoke(_player);

            Debug.Log("Are buttons assigned? 4");
            assignButtons(currentPlayerObj);
            Debug.Log("Are buttons assigned 5?");


        }

        // only works on client
        public GameObject GetPlayerObj()
        {
           return currentPlayerObj;
        }

        public void SetPlayerObj(GameObject _currentPlayerObj)
        {
            currentPlayerObj = _currentPlayerObj;
        }

        public override void OnStartLocalPlayer()
        {
            CmdRequestPlayerObjSpawn();
        }

        [Command] // Request player object to be spawned for client
        void CmdRequestPlayerObjSpawn()
        {
            NetworkSpawnPlayer();
        }

        public void assignButtons(GameObject player)
        {
            _NormalButton = GameObject.Find("AttackButton").GetComponent<Button>();
            _SpecialButton = GameObject.Find("SpecialButton").GetComponent<Button>();
            _UltiButton = GameObject.Find("UltimateButton").GetComponent<Button>();

            //get the action
            UnityAction normalAttack = new UnityAction(player.GetComponent<PlayerAttack>().AttackClick);
            UnityAction specialAttack = new UnityAction(player.GetComponent<PlayerAttack>().SpecialSkillClick);
            UnityAction ultiAttack = new UnityAction(player.GetComponent<PlayerAttack>().UltiClick);

            _NormalButton.onClick.AddListener(normalAttack);
            _SpecialButton.onClick.AddListener(specialAttack);
            _UltiButton.onClick.AddListener(ultiAttack);
            Debug.Log("Are buttons assigned inside?");
        }

        [Server]
        private void NetworkSpawnPlayer()
        {
            GameObject spawn = Instantiate(gameplayPlayerPrefab.gameObject);
            SetPlayerObj(spawn);

            //assign player attack functions to buttons

            NetworkServer.Spawn(spawn, connectionToClient); // pass the client's connection to spawn the player obj prefab for the correct client into any point in the game
            Debug.Log("Are buttons assigned?");
            assignButtons(spawn);
            Debug.Log("Are buttons assigned 3?");
        }


        #region Start & Stop Callbacks

        /// <summary>
        /// Invoked on the server when the object is unspawned
        /// <para>Useful for saving object data in persistent storage</para>
        /// </summary>
        public override void OnStopServer() { }

        /// <summary>
        /// This is invoked on clients when the server has caused this object to be destroyed.
        /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
        /// </summary>
        public override void OnStopClient() { }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { }

        #endregion
    }
}