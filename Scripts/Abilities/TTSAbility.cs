using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TTSAbility : Ability
{
    GameObject kaioKenAura;
    int kaioKenLevel;
    bool kaioKenActive;
    bool startingKaio = false;
    float kaioKenDurationMax;
    float currKaioKenDuration;
    float origDodge;
    float origDamageRed;
    float origStar;
    private void Awake()
    {
        abilityName = "Heavenly Fist";
        animName = "HeavenlyFist";
        kaioKenDurationMax = 10;
    }
    //this is where the magic happens
    public override void BuildAbility()
    {
        base.BuildAbility();
        origDodge = playerStats.dodgeStartUp;
        origDamageRed = playerStats.damageReduction;
        origStar = playerStats.starPunchStartUp;
        player.PlayerDict.Add(Player.PlayerStates.AbilityState, new Action(HeavenlyFist));
    }

    void HeavenlyFist()
    {
        if (player.checkOnce == false && currentCoolDown == coolDownMax)
            StartCoroutine(AbilityCoroutine());
    }
    IEnumerator AbilityCoroutine()
    {
        player.checkOnce = true;
        player.canTakeDamage = false;
        player.ChangeAnimationState(Player.PlayerAnimStates.AbilityState);
        yield return new WaitForSeconds(3);
        player.canTakeDamage = true;
        player.checkOnce = false;
        activeAbilityReady = false;
        player.SetState(Player.PlayerStates.Idle);
    }
    IEnumerator KaioKenCoroutine()
    {
        kaioKenAura.SetActive(true);
        startingKaio = true;
        yield return new WaitForSeconds(1);
        startingKaio = false;
        KaioKenCalcs(kaioKenLevel);
        yield return new WaitForSeconds(.5f);
    }
    void KaioKenCalcs(int kaioLevel)
    {
        //set stats
        kaioKenActive = true;
        switch (kaioLevel)
        {
            case 1:
                playerStats.attackPower *= 2;
                playerStats.currentStamina *= 2;
                playerStats.maxStamina *= 2;
                playerStats.impact *= 2;
                player.maxHealth *= 2;
                player.currentHealth *= 2;
                if (playerStats.dodgeStartUp > 0)
                    playerStats.dodgeStartUp /= 2;
                if (playerStats.damageReduction < 1)
                    playerStats.damageReduction += .1f;
                if (playerStats.starPunchStartUp > 0)
                    playerStats.starPunchStartUp /= 2;
                break;
            case 2:
                playerStats.attackPower *= 4;
                playerStats.currentStamina *= 4;
                playerStats.maxStamina *= 4;
                playerStats.impact *= 4;
                player.maxHealth *= 4;
                player.currentHealth *= 4;
                if (playerStats.dodgeStartUp > 0)
                    playerStats.dodgeStartUp /= 4;
                if (playerStats.damageReduction + .4f < 1)
                    playerStats.damageReduction += .4f;
                else
                    playerStats.damageReduction = 1;
                if (playerStats.starPunchStartUp > 0)
                    playerStats.starPunchStartUp /= 4;
                break;
            case 3:
                playerStats.attackPower *= 20;
                playerStats.currentStamina *= 20;
                playerStats.maxStamina *= 20;
                playerStats.impact *= 20;
                player.maxHealth *= 20;
                player.currentHealth *= 20;
                playerStats.dodgeStartUp = 0;
                playerStats.damageReduction = 1;
                playerStats.starPunchStartUp = 0;
                break;
        }
        //set ui
        uIManager.healthBar.value = player.currentHealth;
        uIManager.staminaBar.value = playerStats.currentStamina;
    }
    void KaioKenCoolOff(int kaioLevel, bool wasKOed)
    {
        //set stats
        kaioKenActive = false;
        kaioKenAura.SetActive(false);
        currKaioKenDuration = kaioKenDurationMax;
        switch (kaioLevel)
        {
            case 1:
                playerStats.attackPower /= 2;
                playerStats.currentStamina /= 2;
                playerStats.maxStamina /= 2;
                playerStats.impact /= 2;
                player.maxHealth /= 2;
                player.currentHealth /= 2;
                playerStats.dodgeStartUp *= 2;
                playerStats.damageReduction -= .1f;
                playerStats.starPunchStartUp *= 2;
                break;
            case 2:
                playerStats.attackPower /= 4;
                playerStats.currentStamina /= 8;
                playerStats.maxStamina /= 4;
                playerStats.impact /= 4;
                player.maxHealth /= 4;
                player.currentHealth /= 8;
                playerStats.dodgeStartUp *= 4;
                playerStats.damageReduction -= .4f;
                playerStats.starPunchStartUp *= 4;
                break;
            case 3:
                playerStats.attackPower /= 20;
                playerStats.currentStamina = 0;
                playerStats.maxStamina /= 20;
                playerStats.impact /= 20;
                player.maxHealth /= 20;
                player.currentHealth = 1;
                playerStats.dodgeStartUp = origDodge;
                playerStats.damageReduction = origDamageRed;
                playerStats.starPunchStartUp = origStar;
                break;
        }
    }
    private void Update()
    {
        if (activeAbilityReady == false)
        {
            currentCoolDown -= Time.deltaTime;
            coolDownText.text = currentCoolDown.ToString();
            if (currentCoolDown <= 0)
            {
                activeAbilityReady = true;
                coolDownText.text = "Ready";
            }
        }
        if(startingKaio == true)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                kaioKenLevel++;
                if (kaioKenLevel > 3)
                    kaioKenLevel = 3;
            }
        }
    }
}
