using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    //References
    PlayerStats playerStats;
    Enemy enemy;
    PauseMan pauseMan;

    //Variables
    //These are the default states for the bools
    public bool? dodgingLeft = null;
    public bool dodgingDown = false;
    bool lowHitStance = true;
    public bool disableInput = false;
    float dodgeCount;
    float dodgeCountMax = 0.2f;
    bool dodgeCountOn = false;
    bool koedDodging = false;
    public bool controlsReversed = false;
    //State Declaration
    public Dictionary<PlayerStates, Action> PlayerDict = new Dictionary<PlayerStates, Action>();
    public PlayerStates currState = PlayerStates.Idle;
    PlayerAnimStates currAnimState = PlayerAnimStates.Idle;



    // Start is called before the first frame update
    public void Start()
    {
        pauseMan = FindObjectOfType<PauseMan>();
        PlayerDict.Add(PlayerStates.Idle, new Action(IdleON));
        PlayerDict.Add(PlayerStates.BlockStun, new Action(BlockStunON));
        PlayerDict.Add(PlayerStates.AttackingL, new Action(() => AttackingON(true)));
        PlayerDict.Add(PlayerStates.AttackingR, new Action(() => AttackingON(false)));
        PlayerDict.Add(PlayerStates.HitStun, new Action(HitStunON));
        PlayerDict.Add(PlayerStates.Blocking, new Action(BlockingON));
        PlayerDict.Add(PlayerStates.Exhausted, new Action(ExhaustionON));
        PlayerDict.Add(PlayerStates.KOPunch, new Action(KOPunchON));
        PlayerDict.Add(PlayerStates.DodgeStartUpL, new Action(() => DodgeStartUpON(true)));
        PlayerDict.Add(PlayerStates.DodgeStartUpR, new Action(() => DodgeStartUpON(false)));
        PlayerDict.Add(PlayerStates.DodgeStartUpD, new Action(DodgeDStartUpON));
        PlayerDict.Add(PlayerStates.KnockDown, new Action(KnockDownON));
        PlayerDict.Add(PlayerStates.TotalKnockOut, new Action(TotalKnockOutON));
        PlayerDict.Add(PlayerStates.TempVictorious, new Action(TempVictoryON));
        PlayerDict.Add(PlayerStates.EnemyBlockStunHitL, new Action(() => BlockStunHitON(true)));
        PlayerDict.Add(PlayerStates.EnemyBlockStunHitR, new Action(() => BlockStunHitON(false)));
        SetState(PlayerStates.Idle);
    }

    private void Update()
    {
        PlayerDict[currState].Invoke();
        if(dodgeCountOn == true)
        {
            dodgeCount -= Time.deltaTime;
            if(dodgeCount <= 0)
            {
                dodgeCountOn = false;
                dodgeCount = dodgeCountMax;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum PlayerStates
    {
        Idle,
        BlockStun,
        AttackingL,
        AttackingR,
        HitStun,
        Blocking,
        Exhausted,
        KOPunch,
        DodgeStartUpL,
        DodgeStartUpR,
        DodgeStartUpD,
        KnockDown,
        TotalKnockOut,
        TempVictorious,
        AbilityState,
        EnemyBlockStunHitL,
        EnemyBlockStunHitR
    }
    public enum PlayerAnimStates
    {
        Idle,
        BlockStun,
        AttackingL,
        AttackingR,
        HitStun,
        Blocking,
        Exhausted,
        KOPunch,
        DodgeStartUpL,
        DodgeStartUpR,
        DodgeStartUpD,
        KnockDown,
        TotalKnockOut,
        TempVictorious,
        AbilityState,
        StayDown,
        GetUp,
        EnemyBlockStunHitL,
        EnemyBlockStunHitR
    }
    public void SetState(PlayerStates newState)
    {
        currState = newState;
    }

    void IdleON()
    {
        if (koedDodging == true)
        {
            if (playerStats.currentKds < playerStats.maxKds)
                SetState(PlayerStates.KnockDown);
            else
                SetState(PlayerStates.TotalKnockOut);
        }
        //set idle graphics  
        canTakeDamage = true;
        ChangeAnimationState(PlayerAnimStates.Idle);
        //All the inputs that can be made while Idle
        InputChecks();
        if (playerStats.currentStamina <= 0)
            SetState(PlayerStates.Exhausted);
    }
    void BlockStunON()
    {
        if(checkOnce == false)
            StartCoroutine(BlockCoroutine());
    }
    void BlockStunHitON(bool isLeft)
    {
        if (checkOnce == false)
            StartCoroutine(BlockHitCoroutine(isLeft));
    }
    void KOPunchON()
    {
        if (checkOnce == false)
            StartCoroutine(KOPunchCoroutine());
    }
    void DodgeStartUpON(bool leftDodge)
    {
        if (checkOnce == false)
            StartCoroutine(Dodge(leftDodge));
    }
    void DodgeDStartUpON()
    {
        if (checkOnce == false)
            StartCoroutine(DodgeD());
    }
    void ExhaustionON()
    {
        ChangeAnimationState(PlayerAnimStates.Exhausted);
        //Dodging
        if (Input.GetKeyDown(KeyCode.A))
            SetState(PlayerStates.DodgeStartUpL);
        if (Input.GetKeyDown(KeyCode.D))
            SetState(PlayerStates.DodgeStartUpR);

        playerStats.currentExhaustionTime -= Time.deltaTime;
        //When exhausted time limit is over refresh variables
        if (playerStats.currentExhaustionTime <= 0)
        {
            playerStats.currentExhaustionTime = playerStats.maxExhaustionTime;
            SetState(PlayerStates.Idle);
            playerStats.currentStamina = playerStats.maxStamina;
            uIManager.staminaBar.value = playerStats.maxStamina;
            uIManager.PHT("Ready to Go!");
        }
    }
    void AttackingON(bool leftPunch)
    {
        if (checkOnce == false)
            StartCoroutine(PunchCoroutine(leftPunch));
    }
    void HitStunON()
    {
        if (checkOnce == false)
            StartCoroutine(HitStunCoroutine());
    }
    void BlockingON()
    {
        //calculations for damage are handled in RecieveDamage
        ChangeAnimationState(PlayerAnimStates.Blocking);
        if (Input.GetKeyUp(KeyCode.S))
        {
            dodgeCountOn = true;
            SetState(PlayerStates.Idle);
        }
    }
    void KnockDownON()
    {
        enemy.SetState(Enemy.EnemyStates.TempVictorious);
        if(canCount == false)
            StartCoroutine(KnockedDown());
        if (canCount == true)
        {
            //play anim
            ChangeAnimationState(PlayerAnimStates.StayDown);
            //Count down and set text
            playerStats.knockDownTime += Time.deltaTime;
            uIManager.knockOutTimer.text = playerStats.knockDownTime.ToString("F0");

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                playerStats.currentDownedInputs++;
        }
        else
            return;

        //Mash requirement
        if (playerStats.currentDownedInputs >= playerStats.maxDownedInputs && checkOnce == false)
            StartCoroutine(GetUpCoroutine());
        //KOOOOOOOOOOO
        if (playerStats.knockDownTime >= playerStats.maxKnockDownTime)
            SetState(PlayerStates.TotalKnockOut);    
    }
    void TotalKnockOutON()
    {
        if (checkOnce == false)
            StartCoroutine(TKOCoroutine());
    }
    protected virtual void TempVictoryON()
    {
        disableInput = true;
        if (enemy.currState == Enemy.EnemyStates.Idle || enemy.currState == Enemy.EnemyStates.BlockingH || enemy.currState == Enemy.EnemyStates.BlockingL)
        {
            disableInput = false;
            SetState(PlayerStates.Idle);
        }      
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////States///////////////////////////////////////////////////////////////////////////////////////////////////



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator PunchCoroutine(bool isLeft)
    {
        checkOnce = true;
        if (isLeft == true)
            ChangeAnimationState(PlayerAnimStates.AttackingL);
        else
            ChangeAnimationState(PlayerAnimStates.AttackingR);
        yield return new WaitForSeconds(playerStats.punchStartUp);
        int thisAttackCritChance;
        //set damage

        //check if attack was interrupted and smart block is on
        if (enemy.currState == Enemy.EnemyStates.AttackingL || enemy.currState == Enemy.EnemyStates.AttackingR)
        {
            if (enemy.smartBlockOn == true)
                enemy.smartBlockOn = false;
        }
        //check for hit
        if (enemy.canTakeDamage == true)
        {
            //Check for Crit
            thisAttackCritChance = UnityEngine.Random.Range(1, 101);
            //Crit
            if (thisAttackCritChance <= playerStats.criticalChance)
            {
                uIManager.PHT("Critical Hit!");
                netDamage = playerStats.attackPower * playerStats.criticalDamage;
                playerStats.impact = playerStats.impact * 3;
                enemy.RecieveDamage(netDamage, playerStats.impact, lowHitStance, false);
                playerStats.impact = playerStats.impact / 3;
                audioManager.PlayClip(AudioManager.ClipNames.CriticalHit, AudioManager.ClipType.SFX);
                Time.timeScale = .5f;
            }
            //not crit
            else
            {
                netDamage = playerStats.attackPower;
                enemy.RecieveDamage(netDamage, playerStats.impact, lowHitStance, false);
            }
            if (enemy.currState == Enemy.EnemyStates.BlockStun)
            {
                if (isLeft == true)
                    SetState(PlayerStates.EnemyBlockStunHitL);
                else
                    SetState(PlayerStates.EnemyBlockStunHitR);
                audioManager.PlayClip(AudioManager.ClipNames.Block, AudioManager.ClipType.SFX);
                checkOnce = false;
            }
            else
            {
                audioManager.PlayClip(AudioManager.ClipNames.Punch, AudioManager.ClipType.SFX);
                yield return new WaitForSeconds(playerStats.punchCoolDown);
                SetState(PlayerStates.Idle);
                Time.timeScale = 1;
                checkOnce = false;
            }
        }
        else
        {
            yield return new WaitForSeconds(playerStats.punchCoolDown);
            SetState(PlayerStates.Idle);
            checkOnce = false;
        }
        
    }
    IEnumerator BlockCoroutine()
    {
        checkOnce = true;
        ChangeAnimationState(PlayerAnimStates.BlockStun);
        yield return new WaitForSeconds(playerStats.blockStun);
        SetState(PlayerStates.Idle);
        checkOnce = false;
    }
    IEnumerator BlockHitCoroutine(bool isLeft)
    {
        checkOnce = true;
        if (isLeft == true)
            ChangeAnimationState(PlayerAnimStates.EnemyBlockStunHitL);
        else
            ChangeAnimationState(PlayerAnimStates.EnemyBlockStunHitR);
        yield return new WaitForSeconds(playerStats.blockStun);
        SetState(PlayerStates.Idle);
        checkOnce = false;
    }
    IEnumerator HitStunCoroutine()
    {
        checkOnce = true;
        ChangeAnimationState(PlayerAnimStates.HitStun);
        audioManager.PlayClip(AudioManager.ClipNames.Punch, AudioManager.ClipType.SFX);
        yield return new WaitForSeconds(playerStats.hitStun);
        SetState(PlayerStates.Idle);
        checkOnce = false;
    }
    IEnumerator KOPunchCoroutine()
    {
        checkOnce = true;
        Enemy enemy = FindObjectOfType<Enemy>();
        ChangeAnimationState(PlayerAnimStates.KOPunch);
        yield return new WaitForSeconds(playerStats.starPunchStartUp);
        //if interuppted
        if (currState != PlayerStates.KOPunch)
        {
            uIManager.PHT("Interrupted!");
            playerStats.starPoints = 0;
            yield break;
        }
        int thisAttackCritChance;
        //set damage
        switch(playerStats.starPoints)
        {
            case 2:
                playerStats.starPunchDamage = playerStats.starPunchDamage * 2;
                break;
            case 3:
                playerStats.starPunchDamage = playerStats.starPunchDamage * 3;
                break;
        }
        playerStats.starPoints = 0;
        uIManager.starPoints.text = playerStats.starPoints.ToString();
        //Check for Crit
        thisAttackCritChance = UnityEngine.Random.Range(1, 101);
        //Crit
        if (thisAttackCritChance <= playerStats.criticalChance)
        {
            uIManager.PHT("Critical Star Punch!");
            netDamage = playerStats.attackPower * playerStats.criticalDamage + playerStats.starPunchDamage;
            enemy.RecieveDamage(netDamage, playerStats.impact, lowHitStance, true);
        }
        //not crit
        else
        {
            uIManager.PHT("Star Punch!");
            netDamage = playerStats.attackPower + playerStats.starPunchDamage;
            enemy.RecieveDamage(netDamage, playerStats.impact, lowHitStance, true);
        }
        audioManager.PlayClip(AudioManager.ClipNames.CriticalHit, AudioManager.ClipType.SFX);
        yield return new WaitForSeconds(playerStats.punchCoolDown);
        SetState(PlayerStates.Idle);
        checkOnce = false;
    }
    IEnumerator DodgeD()
    {
        checkOnce = true;
        ChangeAnimationState(PlayerAnimStates.DodgeStartUpD);
        canTakeDamage = false;
        yield return new WaitForSeconds(playerStats.dodgeDownInvulDuration);
        checkOnce = false;
    }
    IEnumerator Dodge(bool leftDodge)
    {
        checkOnce = true;
        yield return new WaitForSeconds(playerStats.dodgeStartUp);
        if(currState != PlayerStates.DodgeStartUpL )
        {
            if(currState != PlayerStates.DodgeStartUpR)
            {
                checkOnce = false;
                yield break;
            }
        }
        audioManager.PlayClip(AudioManager.ClipNames.Dodge, AudioManager.ClipType.SFX);
        canTakeDamage = false;
        //Checks for direction of dodge
        if (leftDodge == true)
        {
            ChangeAnimationState(PlayerAnimStates.DodgeStartUpL);
            dodgingLeft = true;
        }
        else
        {
            ChangeAnimationState(PlayerAnimStates.DodgeStartUpR);
            dodgingLeft = false;
        }
        if(GameObject.Find("MindMine") != null)
        {
            yield return new WaitForSeconds(playerStats.dodgeInvulDuration / 2);
            checkOnce = true;
            RecieveDamage(FindObjectOfType<SeaSauce>().mindDamage, 0, null, null);
            yield break;
        }
        yield return new WaitForSeconds(playerStats.dodgeInvulDuration);
        checkOnce = false;
        dodgingLeft = null;
        SetState(PlayerStates.Idle);
    }
    IEnumerator KnockedDown()
    {
        ChangeAnimationState(PlayerAnimStates.KnockDown);
        yield return new WaitForSeconds(1);
        canCount = true;
    }
    IEnumerator TKOCoroutine()
    {
        ChangeAnimationState(PlayerAnimStates.StayDown);
        checkOnce = true;
        //for some rreason I couldn't do this in a foreach loop???
        enemy.pointsToGive[0] = 0;
        enemy.pointsToGive[1] = 0;
        enemy.pointsToGive[2] = 0;
        uIManager.knockOutTimer.text = "K.O.!";
        yield return new WaitForSeconds(3);
        FindObjectOfType<WinorLose>().TurnonWinLose(enemy.postBattleQuote[1], false);
    }
    IEnumerator GetUpCoroutine()
    {
        canTakeDamage = false;
        koedDodging = false;
        ChangeAnimationState(PlayerAnimStates.GetUp);
        checkOnce = true;
        //Reset variables
        canCount = false;
        if (FindObjectOfType<TeamThreeStar>() != null)
            FindObjectOfType<TeamThreeStar>().playerOwnedCount.SetActive(false);
        //more inputs for next down
        playerStats.maxDownedInputs = playerStats.maxDownedInputs * 2;
        playerStats.currentDownedInputs = 0;
        playerStats.knockDownTime = 0;
        currentHealth = maxHealth * playerStats.recoveredHealth;
        Mathf.RoundToInt(currentHealth);
        uIManager.healthBar.value = currentHealth;
        uIManager.knockOutTimer.text = "";
        uIManager.PHT("Nice Recovery!");
        yield return new WaitForSeconds(1);
        canTakeDamage = true;
        checkOnce = false;
        SetState(PlayerStates.Idle);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////Enumerators///////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void RecieveDamage(float damageTaken, int recievedImpact, bool? lowHit, bool? starPunch)
    {
        //Dodging
        if (canTakeDamage == false)
        {
            damageTaken = 0;
            uIManager.PHT("Swift Dodge!");
            return;
        }
        canTakeDamage = true;
        if (currState == PlayerStates.KOPunch)
            checkOnce = false;
        //Blocking
        if (currState == PlayerStates.Blocking)
        {
            SetState(PlayerStates.BlockStun);
            audioManager.PlayClip(AudioManager.ClipNames.Block, AudioManager.ClipType.SFX);
            damageTaken = damageTaken * playerStats.damageReduction;
            playerStats.currentStamina -= recievedImpact;
            //Set Text
            uIManager.staminaBar.value = playerStats.currentStamina;
            uIManager.PHT("Valiant Block!");
            if (recievedImpact > playerStats.currentStamina)
                uIManager.PHT("Guard Break!");
        }
        else
            SetState(PlayerStates.HitStun);
        //Set health
        currentHealth = currentHealth - damageTaken;
        Mathf.Round(currentHealth);
        //Set text
        uIManager.healthBar.value = currentHealth;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            uIManager.healthBar.value = currentHealth;
            playerStats.currentKds++;
            uIManager.PHT("Knock Out!");
            if(FindObjectOfType<TeamThreeStar>() != null)
            {
                FindObjectOfType<TeamThreeStar>().playerOwnedCount.SetActive(true);
                FindObjectOfType<TeamThreeStar>().playerOwnedCount.GetComponentInChildren<Text>().text = "Player owned count: " + playerStats.currentKds;
                uIManager.PHT("Complete... annihilation.");
            }
            if(dodgingLeft == true || dodgingLeft == false || dodgingDown == true)
            {
                koedDodging = true;
                return;
            }
            if (playerStats.currentKds < playerStats.maxKds)
                SetState(PlayerStates.KnockDown);
            else
                SetState(PlayerStates.TotalKnockOut);
        }         
    }
    //called by ui manager before player gets turned off
    public void SetStatsandTexts()
    {
        currentHealth = maxHealth;
        uIManager = FindObjectOfType<BattleUIManager>();
        canTakeDamage = true;
        checkOnce = false;
        animator = gameObject.GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        playerStats = GetComponent<PlayerStats>();
        enemy = FindObjectOfType<Enemy>();
        playerStats.currentStamina = playerStats.maxStamina;
        uIManager.starPoints.text = playerStats.starPoints.ToString();
        playerStats.currentExhaustionTime = playerStats.maxExhaustionTime;
        uIManager.healthBar.maxValue = maxHealth;
        uIManager.healthBar.value = maxHealth;
        uIManager.staminaBar.maxValue = playerStats.maxStamina;
        uIManager.staminaBar.value = playerStats.maxStamina;
        CheckForAbilities();
        
    }

    public void PlayerRefresh()
    {
        if (playerStats.currentKds > 0)
            playerStats.currentKds--;
        //Replenish health and Stamina
        currentHealth = currentHealth + maxHealth / 3;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        uIManager.healthBar.value = currentHealth;
        playerStats.currentStamina = playerStats.currentStamina + playerStats.maxStamina / 2;
        if (playerStats.currentStamina > playerStats.maxStamina)
            playerStats.currentStamina = playerStats.maxStamina;
        uIManager.staminaBar.value = playerStats.currentStamina;
    }

    void CheckForAbilities()
    {
        if(AbilityList.allAbilities.Count == 0)
        {
            uIManager.coolDownBackground.SetActive(false);
            return;
        }
        foreach (Ability ability in AbilityList.allAbilities)
        {
            if (ability.isEquipped == true)
            {
                switch(ability.abilityName)
                {
                    case "Pikay's Cowardice":
                        gameObject.AddComponent<PikayAbility>();
                        break;
                }
                GetComponent<Ability>().BuildAbility();
                uIManager.coolDownBackground.SetActive(true);
            }
            else
                uIManager.coolDownBackground.SetActive(false);
        }
    }
    void InputChecks()
    {
        if (pauseMan.willPause == false)
        {
            if (disableInput == false)
            {
                //Blocking
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if(controlsReversed == false)
                    {
                        if (dodgeCountOn == true)
                            SetState(PlayerStates.DodgeStartUpD);
                        else
                            SetState(PlayerStates.Blocking);
                    }
                    else
                        lowHitStance = false;
                }
                //Dodging
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if(controlsReversed == false)
                        SetState(PlayerStates.DodgeStartUpL);
                    else
                        SetState(PlayerStates.DodgeStartUpR);
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if(controlsReversed == false)
                        SetState(PlayerStates.DodgeStartUpR);
                    else
                        SetState(PlayerStates.DodgeStartUpL);
                }
                //Attacking
                if (Input.GetKey(KeyCode.W))
                {
                    if(controlsReversed == false)
                        lowHitStance = false;
                    else if (dodgeCountOn == true)
                        SetState(PlayerStates.DodgeStartUpD);
                    else
                        SetState(PlayerStates.Blocking);

                }
                else
                    lowHitStance = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if(controlsReversed == false)
                        SetState(PlayerStates.AttackingL);
                    else
                        SetState(PlayerStates.AttackingR);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    if(controlsReversed == false)
                        SetState(PlayerStates.AttackingR);
                    else
                        SetState(PlayerStates.AttackingL);
                }
                if (Input.GetKeyDown(KeyCode.Space) && playerStats.starPoints > 0)
                    SetState(PlayerStates.KOPunch);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (GetComponent<Ability>() != null)
                        SetState(PlayerStates.AbilityState);
                }
            }
        }
    }
    public void ChangeAnimationState(PlayerAnimStates newState)
    {
        //stop the same animation from interrupting itself
        if (currAnimState == newState)
            return;
        switch (newState)
        {
            case PlayerAnimStates.Idle:
                tempStateHolder = "Idle";
                break;
            case PlayerAnimStates.AttackingL:
                tempStateHolder = "AttackL";
                break;
            case PlayerAnimStates.AttackingR:
                tempStateHolder = "AttackR";
                break;
            case PlayerAnimStates.Blocking:
                tempStateHolder = "Blocking";
                break;
            case PlayerAnimStates.BlockStun:
                tempStateHolder = "BlockStun";
                break;
            case PlayerAnimStates.DodgeStartUpL:
                tempStateHolder = "DodgeLeft";
                break;
            case PlayerAnimStates.DodgeStartUpR:
                tempStateHolder = "DodgeRight";
                break;
            case PlayerAnimStates.Exhausted:
                tempStateHolder = "Exhausted";
                break;
            case PlayerAnimStates.HitStun:
                tempStateHolder = "HitStun";
                break;
            case PlayerAnimStates.KnockDown:
                tempStateHolder = "KnockDown";
                break;
            case PlayerAnimStates.KOPunch:
                tempStateHolder = "KoPunch";
                break;
            case PlayerAnimStates.TempVictorious:
                tempStateHolder = "Idle";
                break;
            case PlayerAnimStates.TotalKnockOut:
                tempStateHolder = "KnockDown";
                break;
            case PlayerAnimStates.StayDown:
                tempStateHolder = "StayDown";
                break;
            case PlayerAnimStates.GetUp:
                tempStateHolder = "GetUp";
                break;
            case PlayerAnimStates.EnemyBlockStunHitL:
                tempStateHolder = "BlockHitL";
                break;
            case PlayerAnimStates.EnemyBlockStunHitR:
                tempStateHolder = "BlockHitR";
                break;
            case PlayerAnimStates.AbilityState:
                tempStateHolder = GetComponent<Ability>().animName;
                break;
        }
        //Play animation
        animator.Play(tempStateHolder);
        currAnimState = newState;
    }
}
