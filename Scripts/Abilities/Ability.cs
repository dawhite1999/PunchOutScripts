using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public abstract class Ability : MonoBehaviour
{
    public bool isEquipped;
    [SerializeField]
    protected float coolDownMax;
    protected float currentCoolDown;
    protected Player player;
    protected PlayerStats playerStats;
    protected BattleUIManager uIManager;
    protected Text coolDownText;
    protected bool activeAbilityReady = true;
    public string abilityName;
    public string animName;
    public virtual void BuildAbility()
    {
        currentCoolDown = coolDownMax;
        uIManager = FindObjectOfType<BattleUIManager>();
        player = GetComponent<Player>();
        playerStats = GetComponent<PlayerStats>();
        coolDownText = GameObject.Find("CoolDownText").GetComponent<Text>();
        coolDownText.text = "Ready";
    }
}
