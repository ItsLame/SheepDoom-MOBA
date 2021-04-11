﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;

public class PlayerAttachToButtons : NetworkBehaviour
{
    [Header("UI Attack Buttons")]
    [Space(15)]
    public Button _NormalButton;
    public Button _SpecialButton;
    public Button _UltiButton;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority) return;
        assignButtons(this.gameObject);
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
}