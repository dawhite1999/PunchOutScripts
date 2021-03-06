using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class SeaSauce : Enemy
{
    Dictionary<SeaSauceMoveList, Action> SeasauceDict = new Dictionary<SeaSauceMoveList, Action>();
    SeaMoveListAnim currentSeaAnimState;
    SeaSauceMoveList currentSeaState;
    [SerializeField]
    GameObject[] dongs;
    [SerializeField]
    float dongStartup;
    [SerializeField]
    float wwydDuration;
    [SerializeField]
    float mindFieldAnimDuration;
    [SerializeField]
    float mindFieldDuration;
    [SerializeField]
    float dongCoolDown;
    [SerializeField]
    float dongAttack;
    public float mindDamage;
    public GameObject koTimerText;
    GameObject mindMine;
    float koTimer;
    public bool startKOTimer = false;
    public enum SeaSauceMoveList
    {
        Dong,
        WhenWillYouDie,
        MindField,
        Nothing
    }
    public enum SeaMoveListAnim
    {
        Dong,
        WhenWillYouDie,
        MindField,
        Nothing
    }
    public override void Start()
    {
        base.Start();
        SeasauceDict.Add(SeaSauceMoveList.Dong, new Action(DongON));
        SeasauceDict.Add(SeaSauceMoveList.WhenWillYouDie, new Action(WWYDON));
        SeasauceDict.Add(SeaSauceMoveList.MindField, new Action(MindFieldON));
        SeasauceDict.Add(SeaSauceMoveList.Nothing, new Action(Nothing));
        foreach (GameObject dong in dongs)
        {
            dong.SetActive(false);
        }
    }
    void StartMove(SeaSauceMoveList newState)
    {
        inAnotherState = true;
        currentSeaState = newState;
        SeasauceDict[currentSeaState].Invoke();
    }
    protected override void Update()
    {
        base.Update();
        if(startKOTimer == true)
        {
            koTimer -= Time.deltaTime;
            koTimerText.GetComponent<Text>().text = koTimer.ToString();
            if (koTimer <= 0)
            {
                player.currentHealth = 1;
                player.RecieveDamage(999, 0, null, null);
                startKOTimer = false;
                koTimer = 60;
                koTimerText.SetActive(false);
            }
        }
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
        if (attackPicker <= 10)
            SetState(EnemyStates.AttackingL);
        else if (attackPicker > 10 && attackPicker <= 20)
            SetState(EnemyStates.AttackingR);
        else if (attackPicker > 30 && attackPicker <= 80)
            StartMove(SeaSauceMoveList.Dong);
        else if (attackPicker > 80 && attackPicker <= 90)
            StartMove(SeaSauceMoveList.WhenWillYouDie);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void DongON()
    {
        StartCoroutine(Dong());
    }
    void WWYDON()
    {
        StartCoroutine(WWYD());
    }
    void MindFieldON()
    {
        StartCoroutine(MindField());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator Dong()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        int dongPicker = UnityEngine.Random.Range(1, dongs.Length);
        ChangeSeaAnimationState(SeaMoveListAnim.Dong);
        yield return new WaitForSeconds(dongStartup);
        dongs[dongPicker].SetActive(true);
        player.RecieveDamage(enemyStats.attackPower + dongAttack, enemyStats.impact, null, null);
        vulnerable = true;
        yield return new WaitForSeconds(dongCoolDown);
        dongs[dongPicker].SetActive(false);
        vulnerable = false;
        if (currentSeaState != SeaSauceMoveList.Dong)
        {
            koTimer = 60;
            //add star points and set text
            if (FindObjectOfType<PlayerStats>().starPoints < 3)
            {
                FindObjectOfType<PlayerStats>().starPoints++;
                uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                uIManager.PHT("Got Star Point!");
            }
        }
        inAnotherState = false;
    }
    IEnumerator WWYD()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        ChangeSeaAnimationState(SeaMoveListAnim.WhenWillYouDie);
        yield return new WaitForSeconds(wwydDuration);
        koTimer = 60;
        startKOTimer = true;
        koTimerText.SetActive(true);
        inAnotherState = false;
    }
    IEnumerator MindField()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        mindMine.SetActive(true);
        ChangeSeaAnimationState(SeaMoveListAnim.WhenWillYouDie);
        yield return new WaitForSeconds(mindFieldAnimDuration);
        Invoke("CancelMindField", mindFieldDuration);

    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void ChangeSeaAnimationState(SeaMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case SeaMoveListAnim.Dong:
                tempStateHolder = "Dong";
                break;
            case SeaMoveListAnim.MindField:
                tempStateHolder = "MindField";
                break;
            case SeaMoveListAnim.WhenWillYouDie:
                tempStateHolder = "WhenWillYouDie";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentSeaAnimState = newState;
    }
    void CancelMindField()
    {
        mindMine.SetActive(false);
    }
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(SeaSauceMoveList.Nothing);
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
            StartMove(SeaSauceMoveList.Nothing);
            //Add Stagger and set bar vlue
            enemyStats.currentStagger += staggerAdditive;
            uIManager.staggerBar.value = enemyStats.currentStagger;
            //Add repeated hit counter
            if(vulnerable == false)
                SmartBlock(hitLow);
            else
                koTimer = 60;
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
    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
        koTimerText = GameObject.Find("KOTimer");
        mindMine = GameObject.Find("MindMine");
        koTimerText.SetActive(false);
        mindMine.SetActive(false);
    }
}

