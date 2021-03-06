using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PikayAbility : Ability
{
    private void Awake()
    {
        abilityName = "Pikay's Cowardice";
        animName = "PikayCowardice";
    }
    //this is where the magic happens
    public override void BuildAbility()
    {
        base.BuildAbility();
        player.PlayerDict.Add(Player.PlayerStates.AbilityState, new Action(PikayCowardice));
    }

    void PikayCowardice()
    {
        if(player.checkOnce == false && currentCoolDown == coolDownMax)
            StartCoroutine(AbilityCoroutine());
    }
    IEnumerator AbilityCoroutine()
    {
        player.checkOnce = true;
        player.canTakeDamage = false;
        player.ChangeAnimationState(Player.PlayerAnimStates.AbilityState);
        uIManager.PHT("Outtie 5000!");
        yield return new WaitForSeconds(3);
        player.canTakeDamage = true;
        player.checkOnce = false;
        activeAbilityReady = false;
        player.SetState(Player.PlayerStates.Idle);
    }
    private void Update()
    {
        if (activeAbilityReady == false)
        {
            currentCoolDown -= Time.deltaTime;
            coolDownText.text = currentCoolDown.ToString();
            if(currentCoolDown <= 0)
            {
                activeAbilityReady = true;
                coolDownText.text = "Ready";
            }
        }       
    }
}
