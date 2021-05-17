﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class MainMenuUI : NetworkBehaviour
    {
        public Animator transition;
        public void QuitGame()
        {
            Application.Quit(); 
        }

        public void QuitMainMenu()
        {
            NetworkClient.Disconnect();
        }

        public void FadeOut()
        {
            transition.SetTrigger("FadeOut");
        }
}
}
