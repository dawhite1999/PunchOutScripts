using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SuperVillain : Enemy
{
    Dictionary<SVMoveList, Action> SVDict = new Dictionary<SVMoveList, Action>();
    SVMoveListAnim currentAnimState;
    SVMoveList currentSVState;
    GameObject whale;
    [SerializeField]
    float hEStartUp;
    [SerializeField]
    float rCSweetTime;
    [SerializeField]
    float rCDamage;
    [SerializeField]
    int rCImpact;
    [SerializeField]
    float hEDamage;
    [SerializeField]
    int hEImpact;
    [SerializeField]
    float rockClimbStartUp;
    bool rCSweetSpot = false;
    public enum SVMoveList
    {
        HillEngine,
        RockClimb,
        Nothing
    }
    public enum SVMoveListAnim
    {
        HillEngine,
        RockClimb
    }
    public override void Start()
    {
        base.Start();
        SVDict.Add(SVMoveList.HillEngine, new Action(HillEngineOn));
        SVDict.Add(SVMoveList.RockClimb, new Action(RockClimbOn));
        SVDict.Add(SVMoveList.Nothing, new Action(Nothing));
    }
    void StartMove(SVMoveList newState)
    {
        inAnotherState = true;
        currentSVState = newState;
        SVDict[currentSVState].Invoke();
    }
    public override void MakeDecision()
    {
        if (currState == EnemyStates.BlockingH || currState == EnemyStates.BlockingL || currState == EnemyStates.Idle)
            CalculateAttackChances();
    }

    void CalculateAttackChances()
    {
        int attackPicker;
        //pick attack
        attackPicker = UnityEngine.Random.Range(1, 101);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void HillEngineOn()
    {
        StartCoroutine(HillEngineCoroutine());
    }
    void RockClimbOn()
    {
        StartCoroutine(RockClimbCoroutine());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator HillEngineCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        whale.SetActive(true);
        ChangeSVAnimationState(SVMoveListAnim.HillEngine);
        yield return new WaitForSeconds(hEStartUp);
        if (currentAnimState != SVMoveListAnim.HillEngine)
        {
            //add star points and set text
            if (FindObjectOfType<PlayerStats>().starPoints < 3)
            {
                FindObjectOfType<PlayerStats>().starPoints++;
                uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                uIManager.PHT("Got Star Point!");
            }
            whale.SetActive(false);
            uIManager.PHT("Interrupted!");
            inAnotherState = false;
            yield break;
        }
        canTakeDamage = false;
        uIManager.PHT("Hill Engine Blast!");
        player.RecieveDamage(enemyStats.attackPower + hEDamage, enemyStats.impact + hEImpact, null, null);
        audioManager.PlayClip(AudioManager.ClipNames.Kick, AudioManager.ClipType.SFX);
        whale.SetActive(false);
        vulnerable = true;
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        vulnerable = false;
        inAnotherState = false;
    }
    IEnumerator RockClimbCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeSVAnimationState(SVMoveListAnim.RockClimb);
        canTakeDamage = false;
        yield return new WaitForSeconds(rockClimbStartUp);
        canTakeDamage = true;
        vulnerable = true;
        rCSweetSpot = true;
        yield return new WaitForSeconds(rCSweetTime);
        rCSweetSpot = false;
        vulnerable = false;
        canTakeDamage = false;
        if (currentSVState != SVMoveList.RockClimb)
        {
            inAnotherState = false;
            yield break;
        }
        player.RecieveDamage(enemyStats.attackPower + rCDamage, enemyStats.impact + rCImpact, null, null);
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(SVMoveList.Nothing);
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
        else if (smartBlockOn == true && isStaggered == false && rCSweetSpot == false)
        {
            uIManager.PHT("Adapted!");
            damageTaken = Block(damageTaken, lowHit);
        }
        else if (defenceStance == true)
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
            //this is to make sure the movelist resets on hit
            StartMove(SVMoveList.Nothing);
            //Add Stagger and set bar vlue
            enemyStats.currentStagger += staggerAdditive;
            uIManager.staggerBar.value = enemyStats.currentStagger;
            //Add repeated hit counter
            if(rCSweetSpot == false)
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
        if (rCSweetSpot == true)
            currentHealth = 1;
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
    void ChangeSVAnimationState(SVMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case SVMoveListAnim.HillEngine:
                tempStateHolder = "HillEngine";
                break;
            case SVMoveListAnim.RockClimb:
                tempStateHolder = "RockClimb";
                break;
        }
        //Play animation
        animator.Play(tempStateHolder);
        currentAnimState = newState;
    }
    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
        whale = GameObject.Find("Whale");
    }
}
