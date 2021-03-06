using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AnimeLegion : Enemy
{
    Dictionary<ALMoveList, Action> ALDict = new Dictionary<ALMoveList, Action>();
    ALMoveListAnim currentALAnimState;
    ALMoveList currentALState = ALMoveList.Nothing;
    GameObject sluggishLines;
    GameObject paralysisLines;
    [SerializeField]
    int gorrillaDamage;
    [SerializeField]
    int gorrillaImpact;
    [SerializeField]
    float noelStartUp;
    [SerializeField]
    float haachamaStartUp;
    [SerializeField]
    float marineStartUp;
    [SerializeField]
    float pekoraActive;
    bool isPoisoned = false;
    bool isPoisonedStamina = false;
    bool isPoisonedControls = false;
    [SerializeField]
    float poisonDamage;
    [SerializeField]
    float poisonStaminaDamage;
    int poisonTicks = 0;
    int poisonStaminaTicks = 0;
    [SerializeField]
    int timesToRepeat;
    [SerializeField]
    int paralysisDuration;
    public enum ALMoveList
    {
        PekoChanGuard,
        RushiaMagic,
        NoelASMR,
        PirateKiss,
        HaachamaCooking,
        AngelGorrilla,
        Nothing,
        Dizzy,
        UsagiBreathing1stForm
    }
    public enum ALMoveListAnim
    {
        PekoChanGuard,
        RushiaMagic,
        NoelASMR,
        PirateKiss,
        HaachamaCooking,
        AngelGorrilla,
        Dizzy,
        UsagiBreathing1stForm
    }
    public override void Start()
    {
        base.Start();
        ALDict.Add(ALMoveList.AngelGorrilla, new Action(AngelGorrillaON));
        ALDict.Add(ALMoveList.NoelASMR, new Action(NoelASMRON));
        ALDict.Add(ALMoveList.HaachamaCooking, new Action(HaachamaCookingON));
        ALDict.Add(ALMoveList.PekoChanGuard, new Action(PekoChanGuardON));
        ALDict.Add(ALMoveList.PirateKiss, new Action(PirateKissON));
        ALDict.Add(ALMoveList.RushiaMagic, new Action(RushiaMagicON));
        ALDict.Add(ALMoveList.Nothing, new Action(Nothing));
        ALDict.Add(ALMoveList.Dizzy, new Action(DizzyON));
        ALDict.Add(ALMoveList.UsagiBreathing1stForm, new Action(() => UsagiBreathingON(0)));
    }
    void StartMove(ALMoveList newState)
    {
        inAnotherState = true;
        currentALState = newState;
        ALDict[currentALState].Invoke();
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
    void AngelGorrillaON()
    {
        StartCoroutine(AngelGorrila());
    }
    void UsagiBreathingON(float damage)
    {
        StartCoroutine(UsagiBreathing(damage));
    }
    void RushiaMagicON()
    {
        StartCoroutine(RushiaMagic());
    }
    void PekoChanGuardON()
    {
        StartCoroutine(PekoChanGuard());
    }
    void NoelASMRON()
    {
        StartCoroutine(NoelASMR());
    }
    void PirateKissON()
    {
        StartCoroutine(PirateKiss());
    }
    void HaachamaCookingON()
    {
        StartCoroutine(HaachamaCooking());
    }
    void DizzyON()
    {
        StartCoroutine(Dizzy());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator AngelGorrila()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        ChangeALAnimationState(ALMoveListAnim.AngelGorrilla);
        yield return new WaitForSeconds(.5f);
        if (player.dodgingLeft == true || player.dodgingLeft == false)
            player.canTakeDamage = true;
        player.RecieveDamage(enemyStats.attackPower + gorrillaDamage, enemyStats.impact + gorrillaImpact, null, null);
        yield return new WaitForSeconds(.2f);
        inAnotherState = false;
    }
    IEnumerator RushiaMagic()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeALAnimationState(ALMoveListAnim.RushiaMagic);
        canTakeDamage = false;
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 4; i++)
        {
            player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
            yield return new WaitForSeconds(.7f);
        }
        StartMove(ALMoveList.Dizzy);
    }
    IEnumerator Dizzy()
    {
        canTakeDamage = true;
        ChangeALAnimationState(ALMoveListAnim.Dizzy);
        yield return new WaitForSeconds(1);
        if(currState != EnemyStates.Idle)
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
    IEnumerator UsagiBreathing(float damage)
    {
        canTakeDamage = false;
        ChangeALAnimationState(ALMoveListAnim.UsagiBreathing1stForm);
        yield return new WaitForSeconds(.1f);
        player.RecieveDamage(damage * 2, 0, null, null);
        inAnotherState = false;
    }
    IEnumerator PekoChanGuard()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeALAnimationState(ALMoveListAnim.PekoChanGuard);
        yield return new WaitForSeconds(pekoraActive);
        if(currentALState != ALMoveList.UsagiBreathing1stForm)
            inAnotherState = false;
    }
    IEnumerator NoelASMR()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeALAnimationState(ALMoveListAnim.NoelASMR);
        yield return new WaitForSeconds(noelStartUp);
        if (currentALState != ALMoveList.NoelASMR)
        {
            //add star points and set text
            if (FindObjectOfType<PlayerStats>().starPoints < 3)
            {
                FindObjectOfType<PlayerStats>().starPoints++;
                uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                uIManager.PHT("Got Star Point!");
            }
            checkOnce = false;
            uIManager.PHT("Interrupted!");
            inAnotherState = false;
            yield break;
        }
        if (player.currState != Player.PlayerStates.Blocking)
        {
            canTakeDamage = false;
            uIManager.PHT("It feels so good!");
            sluggishLines.SetActive(true);
            FindObjectOfType<PlayerStats>().dodgeStartUp = .5f;
            FindObjectOfType<PlayerStats>().starPunchStartUp *= 2f;
            FindObjectOfType<PlayerStats>().punchStartUp = .5f;
            yield return new WaitForSeconds(1);
            sluggishLines.SetActive(false);
        }
        else
            uIManager.PHT("Ears have been covered!");
        yield return new WaitForSeconds(1);
        inAnotherState = false;
    }
    IEnumerator PirateKiss()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        ChangeALAnimationState(ALMoveListAnim.PirateKiss);
        yield return new WaitForSeconds(marineStartUp);
        if (currentALState != ALMoveList.PirateKiss)
        {
            //add star points and set text
            if (FindObjectOfType<PlayerStats>().starPoints < 3)
            {
                FindObjectOfType<PlayerStats>().starPoints++;
                uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                uIManager.PHT("Got Star Point!");
            }
            checkOnce = false;
            uIManager.PHT("Interrupted!");
            inAnotherState = false;
            yield break;
        }
        if (player.canTakeDamage == true)
        {
            uIManager.PHT("Paralyzed by lust!");
            paralysisLines.SetActive(true);
            player.disableInput = true;
            Invoke("Unparalyze", paralysisDuration);
        }
        yield return new WaitForSeconds(1);
        inAnotherState = false;
    }
    IEnumerator HaachamaCooking()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        yield return new WaitForSeconds(haachamaStartUp);
        if (currentALState != ALMoveList.HaachamaCooking)
        {
            //add star points and set text
            if (FindObjectOfType<PlayerStats>().starPoints < 3)
            {
                FindObjectOfType<PlayerStats>().starPoints++;
                uIManager.starPoints.text = FindObjectOfType<PlayerStats>().starPoints.ToString();
                uIManager.PHT("Got Star Point!");
            }
            uIManager.PHT("Interrupted!");
            inAnotherState = false;
            yield break;
        }
        if (isPoisoned == false)
            InvokeRepeating("PoisonHealth", 0, .5f);
        if (isPoisonedStamina == false)
            InvokeRepeating("PoisonStamina", 0, .5f);
        player.controlsReversed = true;
        canTakeDamage = false;
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void PoisonHealth()
    {
        isPoisoned = true;
        if (player.currentHealth - poisonDamage <= 0)
            player.currentHealth -= 0;
        else
        {
            player.currentHealth -= poisonDamage;
            uIManager.healthBar.value = player.currentHealth;
        }
        poisonTicks++;
        if (poisonTicks == timesToRepeat)
        {
            //I had to undo the reversed controls somewhere, so here it is
            player.controlsReversed = false;
            isPoisoned = false;
            CancelInvoke("PoisonHealth");
        }
    }
    void PoisonStamina()
    {
        isPoisonedStamina = true;
        FindObjectOfType<PlayerStats>().currentStamina -= poisonStaminaDamage;
        uIManager.staminaBar.value = FindObjectOfType<PlayerStats>().currentStamina;
        poisonStaminaTicks++;
        if (poisonStaminaTicks == timesToRepeat)
        {
            isPoisonedStamina = false;
            CancelInvoke("PoisonStamina");
        }
    }
    void Unparalyze()
    {
        player.disableInput = false;
        paralysisLines.SetActive(false);
        uIManager.PHT("Post nut clarity!");
    }
    void ChangeALAnimationState(ALMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case ALMoveListAnim.AngelGorrilla:
                tempStateHolder = "AngelGorrilla";
                break;
            case ALMoveListAnim.Dizzy:
                tempStateHolder = "Dizzy";
                break;
            case ALMoveListAnim.HaachamaCooking:
                tempStateHolder = "HaachamaCooking";
                break;
            case ALMoveListAnim.NoelASMR:
                tempStateHolder = "NoelASMR";
                break;
            case ALMoveListAnim.PekoChanGuard:
                tempStateHolder = "PekoChanGuard";
                break;
            case ALMoveListAnim.PirateKiss:
                tempStateHolder = "PirateKiss";
                break;
            case ALMoveListAnim.RushiaMagic:
                tempStateHolder = "RushiaMagic";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentALAnimState = newState;
    }
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(ALMoveList.Nothing);
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
        //PekoChan Guard
        if(currentALState == ALMoveList.PekoChanGuard)
        {
            StartMove(ALMoveList.UsagiBreathing1stForm);
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
            StartMove(ALMoveList.Nothing);
            //Add Stagger and set bar vlue
            if (currentALState == ALMoveList.Dizzy)
            {
                enemyStats.currentStagger += staggerAdditive * 2;
                damageTaken *= 2;
            }
            else
                enemyStats.currentStagger += staggerAdditive;
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
        sluggishLines = GameObject.Find("SluggishLines");
        paralysisLines = GameObject.Find("ParalysisLines");
        sluggishLines.SetActive(false);
        paralysisLines.SetActive(false);
    }
}
