using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DropYoCombos : Enemy
{
    Dictionary<DYCMoveList, Action> DYCDict = new Dictionary<DYCMoveList, Action>();
    DYCMoveListAnim currentDYCAnimState;
    DYCMoveList currentDYCState;
    GameObject afterImage;
    [SerializeField]
    float trySomethingActive;
    [SerializeField]
    float trySomethingCoolDown;
    [SerializeField]
    float afterImageStartUp;
    [SerializeField]
    float afterImageCoolDown;
    [SerializeField]
    float comboStartUp;
    [SerializeField]
    float comboInterval;
    bool cameFromCombo = false;
    public enum DYCMoveList
    {
        AfterImage,
        TrySomething,
        Combo,
        Counter,
        Dizzy,
        Nothing
    }
    public enum DYCMoveListAnim
    {
        AfterImageL,
        AfterImageR,
        AfterImageD,
        TrySomething,
        Combo,
        Counter,
        Dizzy,
        Nothing
    }

    public override void Start()
    {
        base.Start();
        DYCDict.Add(DYCMoveList.AfterImage, new Action(AfterImageOn));
        DYCDict.Add(DYCMoveList.TrySomething, new Action(TrySomethingOn));
        DYCDict.Add(DYCMoveList.Combo, new Action(ComboOn));
        DYCDict.Add(DYCMoveList.Dizzy, new Action(DizzyON));
        DYCDict.Add(DYCMoveList.Counter, new Action(() => CounterON(0)));
    }
    void StartMove(DYCMoveList newState)
    {
        inAnotherState = true;
        currentDYCState = newState;
        DYCDict[currentDYCState].Invoke();
    }
    void CalculateAttackChances()
    {
        int attackPicker;
        //pick attack
        attackPicker = UnityEngine.Random.Range(1, 101);;
    }
    public override void MakeDecision()
    {
        if (currState == EnemyStates.BlockingH || currState == EnemyStates.BlockingL || currState == EnemyStates.Idle)
            CalculateAttackChances();
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void AfterImageOn()
    {
        StartCoroutine(AfterImage());
    }
    void TrySomethingOn()
    {
        StartCoroutine(TrySomething());
    }
    void ComboOn()
    {
        StartCoroutine(Combo());
    }
    void DizzyON()
    {
        StartCoroutine(Dizzy());
    }
    void CounterON(float damage)
    {
        StartCoroutine(Counter(damage));
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator AfterImage()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        int afterImagePick = UnityEngine.Random.Range(1, 4);
        switch (afterImagePick)
        {
            case 1:
                ChangeDYCAnimationState(DYCMoveListAnim.AfterImageL);
                yield return new WaitForSeconds(afterImageStartUp);
                if (player.dodgingLeft == true)
                    player.canTakeDamage = true;
                break;
            case 2:
                ChangeDYCAnimationState(DYCMoveListAnim.AfterImageR);
                yield return new WaitForSeconds(afterImageStartUp);
                if (player.dodgingLeft == false)
                    player.canTakeDamage = true;
                break;
            case 3:
                ChangeDYCAnimationState(DYCMoveListAnim.AfterImageD);
                yield return new WaitForSeconds(afterImageStartUp);
                if (player.dodgingDown == true)
                    player.canTakeDamage = true;
                break;
        }
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        yield return new WaitForSeconds(afterImageCoolDown);
        if (cameFromCombo == true)
        {
            cameFromCombo = false;
            StartMove(DYCMoveList.Dizzy);
        }
        inAnotherState = false;
    }
    IEnumerator Combo()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        int dropChoice = UnityEngine.Random.Range(1, 3);
        ChangeDYCAnimationState(DYCMoveListAnim.Combo);
        yield return new WaitForSeconds(comboStartUp);
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        yield return new WaitForSeconds(comboInterval);
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        yield return new WaitForSeconds(comboInterval);
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        yield return new WaitForSeconds(comboInterval);
        switch (dropChoice)
        {
            case 1:
                cameFromCombo = true;
                StartMove(DYCMoveList.AfterImage);
                break;
            case 2:
                StartMove(DYCMoveList.Dizzy);
                break;
        }
    }
    IEnumerator TrySomething()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeDYCAnimationState(DYCMoveListAnim.TrySomething);
        yield return new WaitForSeconds(trySomethingActive);
        if (currentDYCState != DYCMoveList.TrySomething)
            inAnotherState = false;
        yield return new WaitForSeconds(trySomethingCoolDown);
        inAnotherState = false;
    }
    IEnumerator Counter(float damage)
    {
        canTakeDamage = false;
        ChangeDYCAnimationState(DYCMoveListAnim.Counter);
        yield return new WaitForSeconds(.1f);
        uIManager.PHT("TRY SOMETHING!");
        player.RecieveDamage(damage * 2, 0, null, null);
        inAnotherState = false;
    }
    IEnumerator Dizzy()
    {
        canTakeDamage = true;
        ChangeDYCAnimationState(DYCMoveListAnim.Dizzy);
        yield return new WaitForSeconds(2);
        if (currState != EnemyStates.Idle)
        {
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
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(DYCMoveList.Nothing);
            //the line below is here to prevent the functionality that happens when you meet the stagger threshold from repeating everytime the enemy is hit in the staggered state
            enemyStats.currentStagger += staggerAdditive;
            enemyStats.currentComboHits++;
            //Set State
            SetState(EnemyStates.HitStun);
        }
        //Dodging
        if (canTakeDamage == false)
        {
            Debug.Log("can't take damage");
            return;
        }
        //TRY SOMETHING!!!
        if (currentDYCState == DYCMoveList.TrySomething)
        {
            StartMove(DYCMoveList.Counter);
            return;
        }
        //Blocking
        else if (smartBlockOn == true && isStaggered == false)
        {
            uIManager.PHT("Adapted!");
            damageTaken = Block(damageTaken, lowHit);
        }
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
            StartMove(DYCMoveList.Nothing);
            //Add Stagger and set bar vlue
            if(currentDYCState == DYCMoveList.Dizzy)
            {
                enemyStats.currentStagger += staggerAdditive * 2;
                damageTaken *= 2;
            }
            else
                enemyStats.currentStagger += staggerAdditive;
            uIManager.staggerBar.value = enemyStats.currentStagger;
            //Add repeated hit counter
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
    void ChangeDYCAnimationState(DYCMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case DYCMoveListAnim.TrySomething:
                tempStateHolder = "TrySomething";
                break;
            case DYCMoveListAnim.AfterImageL:
                tempStateHolder = "AfterImage";
                break;
            case DYCMoveListAnim.Combo:
                tempStateHolder = "Combo";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentDYCAnimState = newState;
    }
    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
        afterImage = GameObject.Find("AfterImage");
        afterImage.SetActive(false);
    }
}
