using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RedHairedHero : Enemy
{
    Dictionary<RHHMoveList, Action> RHHDict = new Dictionary<RHHMoveList, Action>();
    RHHMoveListAnim currentAnimState;
    RHHMoveList currentRHHState;
    [SerializeField]
    float noOptionsStartUp;
    [SerializeField]
    float pizzasStartUp;
    [SerializeField]
    float blueShellStartUp;
    [SerializeField]
    float pizzasInterval;
    [SerializeField]
    float noOptionsCoolDown;
    [SerializeField]
    float mayaDurMax;
    float mayaDurCurr;
    [SerializeField]
    int shellImpact;
    GameObject blueShell;
    GameObject pizzas;
    GameObject MayaGO;
    [SerializeField]
    GameObject[] itemsToPick = new GameObject[5];
    List<GameObject> inventory;
    GameObject inventoryDisplay;
    bool mayaActive = false;

    public enum RHHMoveList
    {
        NoOptions,
        Pizzas,
        Maya,
        BlueShell,
        Point,
        Nothing
    }
    public enum RHHMoveListAnim
    {
        NoOptions,
        Pizzas,
        Maya,
        BlueShell,
        Point,
        Nothing
    }
    public override void Start()
    {
        base.Start();
        RHHDict.Add(RHHMoveList.NoOptions, new Action(NoOptionsOn));
        RHHDict.Add(RHHMoveList.Pizzas, new Action(PizzasOn));
        RHHDict.Add(RHHMoveList.Maya, new Action(MayaOn));
        RHHDict.Add(RHHMoveList.BlueShell, new Action(BlueShellOn));
        RHHDict.Add(RHHMoveList.Point, new Action(PointOn));
        RHHDict.Add(RHHMoveList.Nothing, new Action(Nothing));
        mayaDurCurr = mayaDurMax;
    }
    protected override void Update()
    {
        base.Update();
        if (mayaActive == true)
        {
            mayaDurCurr -= Time.deltaTime;
            if (mayaDurCurr <= 0)
            {
                mayaActive = false;
                MayaGO.GetComponent<Animator>().Play("MayaLeave");
                Invoke("CancelMaya" , 1);
            }
        }
    }
    void StartMove(RHHMoveList newState)
    {
        inAnotherState = true;
        currentRHHState = newState;
        RHHDict[currentRHHState].Invoke();
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
    void NoOptionsOn()
    {
        StartCoroutine(NoOptions());
    }
    void PizzasOn()
    {
        StartCoroutine(Pizzas());
    }
    void MayaOn()
    {
        StartCoroutine(Maya());
    }
    void BlueShellOn()
    {
        StartCoroutine(BlueShell());
    }
    void PointOn()
    {
        StartCoroutine(Point());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator NoOptions()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        canTakeDamage = false;
        ChangeRHHAnimationState(RHHMoveListAnim.NoOptions);
        yield return new WaitForSeconds(noOptionsStartUp);
        player.RecieveDamage(99999, 99999, null, null);
        yield return new WaitForSeconds(noOptionsCoolDown);
        inAnotherState = false;
    }
    IEnumerator Pizzas()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        pizzas.SetActive(true);
        ChangeRHHAnimationState(RHHMoveListAnim.Pizzas);
        yield return new WaitForSeconds(pizzasStartUp);
        if (currentRHHState != RHHMoveList.Pizzas)
        {
            uIManager.PHT("Interrupted!");
            pizzas.SetActive(false);
            yield break;
        }
        canTakeDamage = false;
        uIManager.PHT("Pizzas!");
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        audioManager.PlayClip(AudioManager.ClipNames.Kick, AudioManager.ClipType.SFX);
        yield return new WaitForSeconds(pizzasInterval);
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
        uIManager.PHT("They come back you know!");
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        inAnotherState = false;
    }
    IEnumerator Maya()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        MayaGO.SetActive(true);
        ChangeRHHAnimationState(RHHMoveListAnim.Maya);
        mayaActive = true;
        yield return new WaitForSeconds(1);
        inAnotherState = false;
    }
    IEnumerator BlueShell()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        blueShell.SetActive(true);
        ChangeRHHAnimationState(RHHMoveListAnim.BlueShell);
        yield return new WaitForSeconds(blueShellStartUp);
        if (currentRHHState != RHHMoveList.BlueShell)
        {
            uIManager.PHT("Interrupted!");
            blueShell.SetActive(false);
            inAnotherState = false;
            yield break;
        }
        if (player.dodgingDown == true || player.dodgingLeft == true || player.dodgingLeft == false)
            player.canTakeDamage = true;
        player.RecieveDamage(enemyStats.attackPower, enemyStats.impact + shellImpact, null, null);
        inAnotherState = false;
    }
    IEnumerator Point()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        yield return new WaitForSeconds(1);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void CancelMaya()
    {
        MayaGO.SetActive(false);
        mayaActive = false;
        mayaDurCurr = mayaDurMax;
    }
    /*void GetRandomItem()
    {
        //calculate chance
        int itemPicker = UnityEngine.Random.Range(1, 101);
        //if space and bad evidence
        if (itemPicker <= 80 && inventory.Count < 3)
        {
            itemPicker = UnityEngine.Random.Range(1, 6);
            switch (itemPicker)
            {
                case 1:
                    inventory.Add(itemsToPick[1]);
                    break;
                case 2:
                    inventory.Add(itemsToPick[2]);
                    break;
                case 3:
                    inventory.Add(itemsToPick[3]);
                    break;
                case 4:
                    inventory.Add(itemsToPick[4]);
                    break;
                case 5:
                    inventory.Add(itemsToPick[5]);
                    break;
            }
            //display
            Instantiate(itemsToPick[itemPicker], inventoryDisplay.transform);
        }
        //if space and good evidence
        else if (itemPicker > 80 && inventory.Count < 3)
        {
            inventory.Add(itemsToPick[0]);
            Instantiate(itemsToPick[0], inventoryDisplay.transform);
        }
        //if no space
        if(inventory.Count == 3)
        {

            foreach (GameObject item in inventory)
            {
                if (item.GetComponent<Evidence>().evidenceType != "good")
                {
                    inventory.Remove(item);
                    foreach (GameObject itemDisplay in inventoryDisplay.transform.ch)
                    {
                        if(inventoryDisplay)
                    }
                    GameObject tempItem = inventoryDisplay.GetComponentInChildren<>
                    Destroy()
                    
                }

            }
        }


    }*/
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(RHHMoveList.Nothing);
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
            blueShell.SetActive(false);
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
            blueShell.SetActive(false);
            //this is to make sure the movelist resets on hit
            StartMove(RHHMoveList.Nothing);
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
    void ChangeRHHAnimationState(RHHMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case RHHMoveListAnim.Point:
                tempStateHolder = "KaioKen1";
                break;
            case RHHMoveListAnim.BlueShell:
                tempStateHolder = "KaioKen3";
                break;
            case RHHMoveListAnim.Maya:
                tempStateHolder = "MrPoPo";
                break;
            case RHHMoveListAnim.NoOptions:
                tempStateHolder = "MuffinButton";
                break;
            case RHHMoveListAnim.Pizzas:
                tempStateHolder = "SensuBean";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentAnimState = newState;
    }

    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
        blueShell = GameObject.Find("BlueShell");
        blueShell.SetActive(false);
        pizzas = GameObject.Find("Pizzas");
        pizzas.SetActive(false);
        MayaGO = GameObject.Find("Maya");
        MayaGO.SetActive(false);
        inventoryDisplay = GameObject.Find("InventoryDisplay");
        foreach (GameObject item in itemsToPick)
        {
            item.SetActive(false);
        }
    }
}
