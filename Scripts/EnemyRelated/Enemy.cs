using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : BaseCharacter
{
    //References
    protected EnemyStats enemyStats;
    protected Player player;
    //Variables
    protected int moveSelection;
    protected bool blockingLow = false;
    protected bool blockingHigh = false;
    protected bool isStaggered = false;
    protected bool? hitLow = null;
    protected float maxIdleTime;
    protected bool defenceStance = true;
    protected bool vulnerable = false;
    int knockDownWait = 2;
    int repeatedLowHits;
    int repeatedHighHits;
    float repeatedLowHitTimer;
    float repeatedHighHitTimer;
    public string enemyName;
    public bool inAnotherState = false;
    [HideInInspector]
    public bool smartBlockOn = false;
    [Header("Gamer, Meme, Intelligence is the order from 0 to 2")]
    public int[] pointsToGive = new int[3];
    [Header("0 is a player victory, 1 is a player loss")]
    public string[] postBattleQuote = new string[2];

    //State Declaration
    Dictionary<EnemyStates, Action> EnemyDict = new Dictionary<EnemyStates, Action>();
    public EnemyStates currState = EnemyStates.Idle;
    AnimationStates currentState = AnimationStates.IDLE;


    // Start is called before the first frame update
    public virtual void Start()
    {
        EnemyDict.Add(EnemyStates.Idle, new Action(IdleON));
        EnemyDict.Add(EnemyStates.BlockStun, new Action(BlockStunON));
        EnemyDict.Add(EnemyStates.AttackingL, new Action(() => AttackingON(true)));
        EnemyDict.Add(EnemyStates.AttackingR, new Action(() => AttackingON(false)));
        EnemyDict.Add(EnemyStates.HitStun, new Action(HitStunON));
        EnemyDict.Add(EnemyStates.Taunting, new Action(() => TauntingON()));
        EnemyDict.Add(EnemyStates.BlockingH, new Action(() => BlockingON(false)));
        EnemyDict.Add(EnemyStates.BlockingL, new Action(() => BlockingON(true)));
        EnemyDict.Add(EnemyStates.Staggered, new Action(ComboTimeON));
        EnemyDict.Add(EnemyStates.TempVictorious, new Action(TempVictory));
        EnemyDict.Add(EnemyStates.KnockedDown, new Action(KnockDownON));
        EnemyDict.Add(EnemyStates.TotalKnockOut, new Action(TotalKnockOutON));
        SetState(EnemyStates.Idle);
        player = FindObjectOfType<Player>();
        defenceStance = false;
    }

    protected virtual void Update()
    {
        EnemyDict[currState].Invoke();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////States///////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum EnemyStates
    {
        Idle,
        BlockStun,
        AttackingL,
        AttackingR,
        HitStun,
        Taunting,
        BlockingH,
        BlockingL,
        Staggered,
        TempVictorious,
        KnockedDown,
        TotalKnockOut
    }
    //Animation States
    protected enum AnimationStates
    {
        GETUPSUCCESS,
        GETUPTRY,
        HIGHBLOCK,
        HIGHIDLE,
        HITHIGH,
        HITLOW,
        IDLE,
        KD,
        LEFTPUNCH,
        LOWBLOCK,
        LOWIDLE,
        RIGHTPUNCH,
        STAGGERED,
        STAYDOWN,
        TAUNT,
        VICTORY
    }
    public void SetState(EnemyStates newState)
    {
        currState = newState;
    }

    protected virtual void IdleON()
    {
        if(inAnotherState == false)
        {
            //Set Idle Anim
            ChangeAnimationState(AnimationStates.IDLE);
            //pick a random blocking stance
            int blockChoice = UnityEngine.Random.Range(0, 2);
            canTakeDamage = true;
            countTime = true;

            if (blockChoice == 0)
                SetState(EnemyStates.BlockingL);
            else
                SetState(EnemyStates.BlockingH);
        }
    }
    protected virtual void BlockStunON()
    {
        if (checkOnce == false)
            StartCoroutine(BlockCoroutine());
    }
    protected virtual void AttackingON(bool leftPunch)
    {
        if (checkOnce == false)
            StartCoroutine(PunchCoroutineE(leftPunch)); ;
    }
    protected virtual void HitStunON()
    {
        if (checkOnce == false)
            StartCoroutine(HitStunCoroutine());
    }
    protected virtual void TempVictory()
    {
        if (checkOnce == false)
        {
            //play victory animation
            ChangeAnimationState(AnimationStates.VICTORY);
        }
        if (player.currState != Player.PlayerStates.KnockDown)
            SetState(EnemyStates.Idle);
    }
    protected virtual void AttackInvulON()
    {
        canTakeDamage = false;
    }
    protected virtual void TauntingON()
    {
        if (checkOnce == false)
            StartCoroutine(TauntCoroutine());
    }
    protected virtual void ComboTimeON()
    {
        enemyStats.currentComboTime -= Time.deltaTime;
        if (enemyStats.currentComboTime <= 0)
        {
            //refresh variables
            RefreshStaggerVars();
            SetState(EnemyStates.Idle);
        }
    }
    protected virtual void BlockingON(bool stanceLow)
    {
        if (countTime == true)
            enemyStats.currentIdleTime -= Time.deltaTime;
        if (stanceLow == true)
            ChangeAnimationState(AnimationStates.LOWIDLE);
        else
            ChangeAnimationState(AnimationStates.HIGHIDLE);
        CountDownSmartTimer();
        if (checkOnce == false && enemyStats.currentIdleTime <= 0)
        {
            if (currState == EnemyStates.BlockingH || currState == EnemyStates.BlockingL)
            {
                //Stop counting and reset current Idle time
                countTime = false;
                maxIdleTime = UnityEngine.Random.Range(enemyStats.maxIdleTimeMin, enemyStats.maxIdleTimeCap + 1);
                enemyStats.currentIdleTime = maxIdleTime;
                MakeDecision();
            }
        }
    }
    void KnockDownON()
    {
        //play the knock out anim once and set player state once
        if (stayDown == false)
        {
            player.SetState(Player.PlayerStates.TempVictorious);
            StartCoroutine(KnockedDown());
        }
        if (canCount == true)
        {
            //Count down and set text
            enemyStats.knockDownTime += Time.deltaTime;
            uIManager.knockOutTimer.text = enemyStats.knockDownTime.ToString("F0");
            //getting up
            if (checkOnce == false)
                StartCoroutine(TryGetUp());
        }
        else
            return;
        //KOOOOOOOOOOO
        if (enemyStats.knockDownTime >= enemyStats.maxKnockDownTime || TKOCheck() == true)
        {
            uIManager.knockOutTimer.text = "K.O.!";
            SetState(EnemyStates.TotalKnockOut);
        }
    }
    void TotalKnockOutON()
    {
        if (checkOnce == false)
            StartCoroutine(TKOCoroutine());
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////States/////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////IEnumerators/////////////////////////////////////////////////////////////////////////////////////////////
    protected virtual IEnumerator PunchCoroutineE(bool isLeft)
    {
        //Set anim
        checkOnce = true;
        if (isLeft == true)
            ChangeAnimationState(AnimationStates.LEFTPUNCH);
        else
            ChangeAnimationState(AnimationStates.RIGHTPUNCH);
        yield return new WaitForSeconds(enemyStats.punchWindUp);
        if (currState != EnemyStates.AttackingL)
        {
            if (currState != EnemyStates.AttackingR)
            {
                uIManager.PHT("Interrupted!");
                yield break;
            }
        }
        canTakeDamage = false;
        //set damage
        netDamage = enemyStats.attackPower;
        player.RecieveDamage(netDamage, enemyStats.impact, null, null);
        vulnerable = true;
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        vulnerable = false;
        if (currState != EnemyStates.AttackingL || currState != EnemyStates.AttackingR)
            yield break;
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    protected IEnumerator BlockCoroutine()
    {
        checkOnce = true;
        //Set Anim
        if (hitLow == true)
            ChangeAnimationState(AnimationStates.LOWBLOCK);
        else
            //Set high block anim
            ChangeAnimationState(AnimationStates.HIGHBLOCK);
        yield return new WaitForSeconds(enemyStats.blockStun);
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    protected IEnumerator HitStunCoroutine()
    {
        checkOnce = true;
        //The last hit of a combo
        if (enemyStats.currentComboHits == enemyStats.maxComboHits)
        {
            canTakeDamage = false;
            uIManager.PHT("Combo Finisher!");
            Time.timeScale = 0.7f;
            //Play hit anim
            if (hitLow == true)
                ChangeAnimationState(AnimationStates.HITLOW);
            else
                ChangeAnimationState(AnimationStates.HITHIGH);
            yield return new WaitForSeconds(enemyStats.hitStun);
            Time.timeScale = 1;
            //Refresh variables
            RefreshStaggerVars();
            SetState(EnemyStates.Idle);
            GetComponent<Renderer>().material.color = Color.yellow;
            checkOnce = false;
            yield break;
        }
        //if stagger requirement is met.
        if (enemyStats.currentStagger == enemyStats.maxStagger)
        {
            isStaggered = true;
            uIManager.PHT("Stagger!");
            uIManager.enemyStagger.text = "Staggered!";
            Time.timeScale = 0.8f;
            //set state
            SetState(EnemyStates.Staggered);
            canTakeDamage = false;
            uIManager.enemyStagger.GetComponent<Animator>().Play("StaggerTextPop");
            audioManager.PlayClip(AudioManager.ClipNames.Stagger, AudioManager.ClipType.SFX);
            //Play hit anim
            if (hitLow == true)
                ChangeAnimationState(AnimationStates.HITLOW);
            else
                ChangeAnimationState(AnimationStates.HITHIGH);
            yield return new WaitForSeconds(enemyStats.hitStun);
            Time.timeScale = 1;
            canTakeDamage = true;
            checkOnce = false;
            //Play stagger anim
            ChangeAnimationState(AnimationStates.STAGGERED);
            yield break;
        }
        //Normal functionality
        canTakeDamage = false;
        //Play hit anim
        if (hitLow == true)
            ChangeAnimationState(AnimationStates.HITLOW);
        else
            ChangeAnimationState(AnimationStates.HITHIGH);
        yield return new WaitForSeconds(enemyStats.comboFinisherHitStun);
        if (enemyStats.currentStagger >= enemyStats.maxStagger)
        {
            SetState(EnemyStates.Staggered);
            ChangeAnimationState(AnimationStates.STAGGERED);
        }
        else
            SetState(EnemyStates.Idle);
        canTakeDamage = true;
        checkOnce = false;
    }
    protected virtual IEnumerator TauntCoroutine()
    {
        checkOnce = true;
        ChangeAnimationState(AnimationStates.TAUNT);
        yield return new WaitForSeconds(2);
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    IEnumerator KnockedDown()
    {
        stayDown = true;
        ChangeAnimationState(AnimationStates.KD);
        yield return new WaitForSeconds(3);
        canCount = true;
    }
    IEnumerator TryGetUp()
    {
        int getUpAttempts;
        //check if it is a TKO
        if (enemyStats.sameRoundKds == enemyStats.sameRoundMaxKds || enemyStats.currentKDs == enemyStats.maxKds)
        {
            SetState(EnemyStates.TotalKnockOut);
            yield break;
        }
        checkOnce = true;
        //Calculate how many get up attempts there will be
        getUpAttempts = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < getUpAttempts; i++)
        {
            //keep enemy on the ground
            ChangeAnimationState(AnimationStates.STAYDOWN);
            //decide a random amount of seconds to space out get up attempt animations
            knockDownWait = UnityEngine.Random.Range(1, 3);
            yield return new WaitForSeconds(knockDownWait);
            //if Succeeded and its the last attempt
            if (i == getUpAttempts - 1 && TKOCheck() == false)
            {
                canCount = false;
                //Play get up full animation
                ChangeAnimationState(AnimationStates.GETUPSUCCESS);
                uIManager.PHT("Recovered!");
                yield return new WaitForSeconds(1);
                //reset values
                enemyStats.knockDownTime = 0;
                currentHealth = maxHealth * enemyStats.recoveredHealth;
                Mathf.RoundToInt(currentHealth);
                uIManager.enemyHealthBar.value = currentHealth;
                uIManager.knockOutTimer.text = "";
                stayDown = false;
                checkOnce = false;
                SetState(EnemyStates.Idle);
                yield break;
            }
            //Play get up partial animation
            ChangeAnimationState(AnimationStates.GETUPTRY);
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator TKOCoroutine()
    {
        checkOnce = true;
        ChangeAnimationState(AnimationStates.STAYDOWN);
        uIManager.knockOutTimer.text = "K.O.!";
        yield return new WaitForSeconds(3);
        FindObjectOfType<WinorLose>().TurnonWinLose(postBattleQuote[0], true);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////IEnumerators////////////////////////////////////////////////////////////////////////////////////////////////////
    public virtual void MakeDecision()
    {
        Debug.Log("Made decision");
    }
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //the line below is here to prevent the functionality that happens when you meet the stagger threshold from repeating everytime the enemy is hit in the staggered state
            enemyStats.currentStagger += staggerAdditive;
            enemyStats.currentComboHits++;
            //Set State
            SetState(EnemyStates.HitStun);
        }
        //Dodging
        if (canTakeDamage == false)
        {
            damageTaken = 0;
            Debug.Log("can't take damage");
            return;
        }
        //Blocking
        else if (smartBlockOn == true && isStaggered == false && vulnerable == false)
        {
            uIManager.PHT("Adapted!");
            damageTaken = Block(damageTaken, lowHit);
        }
        else if(defenceStance == true)
            damageTaken = Block(damageTaken, lowHit);
        //low block
        else if (currState == EnemyStates.BlockingL && lowHit == true && starPunch == false)
            damageTaken = Block(damageTaken, lowHit);
        //high block
        else if (currState == EnemyStates.BlockingH && lowHit == false && starPunch == false)
            damageTaken = Block(damageTaken, lowHit);
        //not staggered
        else if (isStaggered == false)
        {
            //Add Stagger and set bar vlue
            enemyStats.currentStagger += staggerAdditive;
            uIManager.staggerBar.value = enemyStats.currentStagger;
            //Add repeated hit counter
            if (vulnerable == false)
                SmartBlock(hitLow);
            //If an attack was interrupted
            if (currState == EnemyStates.AttackingL || currState == EnemyStates.AttackingR)
            {
                //add star points and set text
                if (FindObjectOfType<PlayerStats>().starPoints < 3)
                {
                    FindObjectOfType<PlayerStats>().starPoints++;
                    uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                    uIManager.PHT("Got Star Point!");
                }
                checkOnce = false;
            }
            //Set State
            SetState(EnemyStates.HitStun);
        }
        //Set health
        currentHealth = currentHealth - damageTaken;
        Mathf.Round(currentHealth);
        //Set text
        uIManager.enemyHealthBar.value = currentHealth;
        //if knocked out
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            uIManager.enemyHealthBar.value = currentHealth;
            enemyStats.currentKDs++;
            enemyStats.sameRoundKds++;
            RefreshStaggerVars();
            uIManager.PHT("Knock Out!");
            SetState(EnemyStates.KnockedDown);
        }
    }
    protected void RefreshStaggerVars()
    {
        enemyStats.currentStagger = 0;
        enemyStats.currentComboHits = 0;
        enemyStats.currentComboTime = enemyStats.maxComboTime;
        uIManager.staggerBar.value = enemyStats.currentStagger;
        uIManager.enemyStagger.text = "";
        isStaggered = false;
    }
    public void SetEnemyStats()
    {
        //Set default color and texts
        currentHealth = maxHealth;
        uIManager = FindObjectOfType<BattleUIManager>();
        canTakeDamage = true;
        checkOnce = false;
        animator = gameObject.GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        enemyStats = GetComponent<EnemyStats>();
        uIManager.enemyHealthBar.maxValue = maxHealth;
        uIManager.enemyHealthBar.value = maxHealth;
        uIManager.staggerBar.maxValue = enemyStats.maxStagger;
        uIManager.staggerBar.value = enemyStats.currentStagger;

        //Set timers
        enemyStats.currentComboTime = enemyStats.maxComboTime;
        maxIdleTime = UnityEngine.Random.Range(enemyStats.maxIdleTimeMin, enemyStats.maxIdleTimeCap + 1);
        enemyStats.currentIdleTime = maxIdleTime;
        repeatedHighHitTimer = enemyStats.repeatedHighHitTimeMax;
        repeatedLowHitTimer = enemyStats.repeatedLowHitTimeMax;
    }
    public void EnemyRefresh()
    {
        RefreshStaggerVars();
        currentHealth = currentHealth + maxHealth / 4;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        uIManager.enemyHealthBar.value = currentHealth;
        enemyStats.currentStagger = 0;
        uIManager.staggerBar.value = enemyStats.currentStagger;
    }
    private bool TKOCheck()
    {
        if (enemyStats.currentKDs == enemyStats.maxKds || enemyStats.sameRoundKds == enemyStats.sameRoundMaxKds)
            return true;
        else
            return false;
    }
    protected void SmartBlock(bool? lowHit)
    {
        if (lowHit == true)
        {
            repeatedLowHits++;
            repeatedLowHitTimer = enemyStats.repeatedLowHitTimeMax;
        }
        else
        {
            repeatedHighHits++;
            repeatedHighHitTimer = enemyStats.repeatedHighHitTimeMax;
        }
        if (repeatedLowHits >= enemyStats.repeatedLowHitsMax || repeatedHighHits >= enemyStats.repeatedHighHitsMax)
            smartBlockOn = true;
    }
    protected float Block(float damage, bool? lowHit)
    {
        //enemy in block stun
        SetState(EnemyStates.BlockStun);
        //calculate damage taken
        damage = damage * enemyStats.damageReduction;
        //reduce player stamina and set text
        FindObjectOfType<PlayerStats>().currentStamina--;
        uIManager.staminaBar.value = FindObjectOfType<PlayerStats>().currentStamina;
        if (FindObjectOfType<PlayerStats>().currentStamina <= 0)
            uIManager.PHT("Exhausted!");
        //Reduce stagger and set text
        if (enemyStats.currentStagger > 0)
        {
            enemyStats.currentStagger -= enemyStats.staggerReduction;
            uIManager.staggerBar.value = enemyStats.currentStagger;
        }
        //Set Text
        uIManager.staminaBar.value = FindObjectOfType<PlayerStats>().currentStamina;
        uIManager.PHT("Valiant Block!");
        //reset smart block variables
        repeatedLowHits = 0;
        repeatedLowHitTimer = enemyStats.repeatedLowHitTimeMax;
        repeatedHighHits = 0;
        repeatedHighHitTimer = enemyStats.repeatedHighHitTimeMax;
        if (smartBlockOn == true)
            smartBlockOn = false;
        return damage;
    }
    void CountDownSmartTimer()
    {
        if (repeatedLowHits > 0)
        {
            repeatedLowHitTimer -= Time.deltaTime;
            if (repeatedLowHitTimer <= 0)
            {
                repeatedLowHits = 0;
                repeatedLowHitTimer = enemyStats.repeatedLowHitTimeMax;
                return;
            }
        }
        if (repeatedHighHits > 0)
        {
            repeatedHighHitTimer -= Time.deltaTime;
            if (repeatedHighHitTimer <= 0)
            {
                repeatedHighHits = 0;
                repeatedHighHitTimer = enemyStats.repeatedHighHitTimeMax;
                return;
            }
        }
    }
    public virtual void SetSpecificEnemy()
    {

    }
    protected void ChangeAnimationState(AnimationStates newState)
    {
        //stop the same animation from interrupting itself
        if (currentState == newState)
            return;
        switch (newState)
        {
            case AnimationStates.IDLE:
                tempStateHolder = "Idle";
                break;
            case AnimationStates.GETUPSUCCESS:
                tempStateHolder = "GetUpSucceed";
                break;
            case AnimationStates.GETUPTRY:
                tempStateHolder = "GetUpTry";
                break;
            case AnimationStates.HIGHBLOCK:
                tempStateHolder = "HighBlock";
                break;
            case AnimationStates.HIGHIDLE:
                tempStateHolder = "HighIdle";
                break;
            case AnimationStates.HITHIGH:
                tempStateHolder = "HitHigh";
                break;
            case AnimationStates.HITLOW:
                tempStateHolder = "HitLow";
                break;
            case AnimationStates.KD:
                tempStateHolder = "KD";
                break;
            case AnimationStates.LEFTPUNCH:
                tempStateHolder = "LeftPunch";
                break;
            case AnimationStates.LOWBLOCK:
                tempStateHolder = "LowBlock";
                break;
            case AnimationStates.LOWIDLE:
                tempStateHolder = "LowIdle";
                break;
            case AnimationStates.RIGHTPUNCH:
                tempStateHolder = "RightPunch";
                break;
            case AnimationStates.STAGGERED:
                tempStateHolder = "Staggered";
                break;
            case AnimationStates.STAYDOWN:
                tempStateHolder = "StayDown";
                break;
            case AnimationStates.TAUNT:
                tempStateHolder = "Taunt";
                break;
            case AnimationStates.VICTORY:
                tempStateHolder = "Victory";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentState = newState;
    }
}

