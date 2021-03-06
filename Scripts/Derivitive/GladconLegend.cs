using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GladconLegend : Enemy
{
    Dictionary<GladconMoveList, Action> GladDict = new Dictionary<GladconMoveList, Action>();
    GladMoveListAnim currentGladAnimState;
    GladconMoveList currentGladState;
    [SerializeField]
    float quickPunchStartUp;
    [SerializeField]
    float quickPunchCoolDown;
    int consecutiveQuicks;
    [SerializeField]
    int consecutiveQuicksMax;
    public enum GladconMoveList
    {
        QuickPunchL,
        QuickPunchR,
        Dizzy,
        Nothing
    }
    public enum GladMoveListAnim
    {
        QuickPunchL,
        QuickPunchR,
        Dizzy,
        Nothing
    }
    public override void Start()
    {
        base.Start();
        GladDict.Add(GladconMoveList.QuickPunchL, new Action(() => QuickPunchON(true)));
        GladDict.Add(GladconMoveList.QuickPunchR, new Action(() => QuickPunchON(false)));
        GladDict.Add(GladconMoveList.Dizzy, new Action(DizzyON));
        GladDict.Add(GladconMoveList.Nothing, new Action(Nothing));
        defenceStance = true;
    }
    void StartMove(GladconMoveList newState)
    {
        inAnotherState = true;
        currentGladState = newState;
        GladDict[currentGladState].Invoke();
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
            StartMove(GladconMoveList.QuickPunchL);
        else if (attackPicker > 80 && attackPicker <= 90)
            StartMove(GladconMoveList.QuickPunchR);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void QuickPunchON(bool isLeft)
    {
        StartCoroutine(QuickPunch(isLeft));
    }
    void DizzyON()
    {
        StartCoroutine(DizzyCoroutine());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator QuickPunch(bool isLeft)
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        consecutiveQuicks++;
        //Set anim
        canTakeDamage = false;
        if (isLeft == true)
            ChangeGladAnimationState(GladMoveListAnim.QuickPunchL);
        else
            ChangeGladAnimationState(GladMoveListAnim.QuickPunchR);
        yield return new WaitForSeconds(quickPunchStartUp);
        //set damage
        netDamage = enemyStats.attackPower;
        player.RecieveDamage(netDamage, enemyStats.impact, null, null);
        yield return new WaitForSeconds(quickPunchCoolDown);
        if (consecutiveQuicks == consecutiveQuicksMax)
            StartMove(GladconMoveList.Dizzy);
    }
    IEnumerator DizzyCoroutine()
    {
        canTakeDamage = true;
        defenceStance = false;
        ChangeGladAnimationState(GladMoveListAnim.Dizzy);
        yield return new WaitForSeconds(1);
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
        defenceStance = true;
    }
    protected override IEnumerator PunchCoroutineE(bool isLeft)
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
        //when the enemy is vulerable
        defenceStance = false;
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        defenceStance = true;
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void ChangeGladAnimationState(GladMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case GladMoveListAnim.QuickPunchL:
                tempStateHolder = "QuickPunchL";
                break;
            case GladMoveListAnim.QuickPunchR:
                tempStateHolder = "QuickPunchR";
                break;
            case GladMoveListAnim.Dizzy:
                tempStateHolder = "Dizzy";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentGladAnimState = newState;
    }

    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(GladconMoveList.Nothing);
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
        else if (smartBlockOn == true && isStaggered == false)
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
            StartMove(GladconMoveList.Nothing);
            //Add Stagger and set bar vlue
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
    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
    }
}
