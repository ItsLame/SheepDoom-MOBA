﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SheepDoom
{
    public class LoginRegister : MonoBehaviour
    {
        [SerializeField] NetworkManager networkManager;
        public InputField username;
        public InputField password;

        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isBatchMode) //Headless build
                Debug.Log($"=== Client Build ===");
            else
            {
                Debug.Log($"=== Server Build ===");
                networkManager.StartServer();
            }
        }

        public void Login()
        {
            string user = username.text;
            if(ClientName.ClientLogin(user)) // server is inactive, so this is only assigned on client at first
            {
                networkManager.networkAddress = "localhost";
                networkManager.StartClient(); 
            }
        }

        public void Register() { }
    }
}
