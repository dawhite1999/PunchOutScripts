using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AnimeMan : Enemy
{
    Dictionary<AnimeManMoveList, Action> AnimeDict = new Dictionary<AnimeManMoveList, Action>();
    AnimeMoveListAnim currentAnimeAnimState;
    AnimeManMoveList currentAnimeState;
    GameObject swordBeam;
    GameObject dice;
    //variables
    bool powerChanged = false;
    float attackOriginal;
    [SerializeField]
    int barryImpact;
    [SerializeField]
    float tetsuDamage;
    [SerializeField]
    float tetsuStartUp;
    [SerializeField]
    float geoStartUp;
    [SerializeField]
    int timesToRepeat;
    [SerializeField]
    int fireDamage;
    int fDOriginal;
    [SerializeField]
    float iceDamage;
    float iDOriginal;
    int fireTicks = 0;
    int iceTicks = 0;
    bool inFire = false;
    bool inIce = false;
    public enum AnimeManMoveList
    {
        Room,
        Geography,
        Roullette,
        TetsugaTenshou,
        Nothing
    }
    public enum AnimeMoveListAnim
    {
        RoomLeft,
        RoomRight,
        RoomTakt,
        Geography,
        Roullette,
        TetsugaTenshou,
        Nothing
    }
    public override void Start()
    {
        base.Start();
        AnimeDict.Add(AnimeManMoveList.Room, new Action(RoomOn));
        AnimeDict.Add(AnimeManMoveList.Geography, new Action(GeographyOn));
        AnimeDict.Add(AnimeManMoveList.Roullette, new Action(RoulletteOn));
        AnimeDict.Add(AnimeManMoveList.TetsugaTenshou, new Action(TetsugaTenshouOn));
        AnimeDict.Add(AnimeManMoveList.Nothing, new Action(Nothing));
        attackOriginal = enemyStats.attackPower;
        fDOriginal = fireDamage;
        iDOriginal = iceDamage;
    }
    void StartMove(AnimeManMoveList newState)
    {
        inAnotherState = true;
        currentAnimeState = newState;
        AnimeDict[currentAnimeState].Invoke();
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
        else if (attackPicker > 20 && attackPicker <= 30)
            StartMove(AnimeManMoveList.TetsugaTenshou);
        else if (attackPicker > 30 && attackPicker <= 80)
            StartMove(AnimeManMoveList.Room);
        else if (attackPicker > 80 && attackPicker <= 90)
            StartMove(AnimeManMoveList.Geography);
        else if (attackPicker > 90 && attackPicker <= 100)
            StartMove(AnimeManMoveList.Roullette);
    }
    protected override void Update()
    {
        base.Update();
        if(inFire == true)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                fireDamage++;
                Debug.Log(fireDamage);
            }

        }
        if(inIce == true)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                iceDamage -= .1f;
                Debug.Log(iceDamage);
                if (iceDamage < .1f)
                    iceDamage = .1f;
            }
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void RoomOn()
    {
        StartCoroutine(RoomCoroutine());
    }
    void GeographyOn()
    {
        StartCoroutine(GeographyCoroutine());
    }
    void RoulletteOn()
    {
        StartCoroutine(RoulletteCoroutine());
    }
    void TetsugaTenshouOn()
    {
        StartCoroutine(TetsuCoroutine());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator RoomCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        int roomAttack = UnityEngine.Random.Range(1, 4);
        switch (roomAttack)
        {
            case 1:
                Debug.Log("L");
                ChangeAnimeAnimationState(AnimeMoveListAnim.RoomLeft);
                yield return new WaitForSeconds(1);
                if (player.dodgingLeft == false)
                    player.canTakeDamage = true;
                player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
                break;
            case 2:
                Debug.Log("R");
                ChangeAnimeAnimationState(AnimeMoveListAnim.RoomRight);
                yield return new WaitForSeconds(1);
                if (player.dodgingLeft == true)
                    player.canTakeDamage = true;
                player.RecieveDamage(enemyStats.attackPower, enemyStats.impact, null, null);
                break;
            case 3:
                Debug.Log("Takt!");
                ChangeAnimeAnimationState(AnimeMoveListAnim.RoomTakt);
                yield return new WaitForSeconds(1);
                player.RecieveDamage(enemyStats.attackPower, enemyStats.impact + barryImpact, null, null);
                break;
        }
        yield return new WaitForSeconds(.2f);
        inAnotherState = false;
    }
    IEnumerator GeographyCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        int geographySelection = UnityEngine.Random.Range(1, 3);
        yield return new WaitForSeconds(geoStartUp);
        if (currentAnimeState != AnimeManMoveList.Geography)
        {
            uIManager.PHT("Interrupted!");
            yield break;
        }
        switch (geographySelection)
        {
            case 1:
                Debug.Log("fire");
                if (inFire == false)
                    InvokeRepeating("GeographyChangeF", 0, .5f);
                else
                {
                    uIManager.PHT("Nothing happened...");
                    break;
                }
                if (inIce == true)
                    CancelInvoke("GeographyChangeI");
                break;
            case 2:
                Debug.Log("ice");
                if(inIce == false)
                    InvokeRepeating("GeographyChangeI", 0, 2f);
                else
                {
                    uIManager.PHT("Nothing happened...");
                    break;
                }
                if (inFire == true)
                    CancelInvoke("GeographyChangeF");
                break;
        }
        inAnotherState = false;
    }
    IEnumerator TetsuCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        swordBeam.SetActive(true);
        ChangeAnimeAnimationState(AnimeMoveListAnim.TetsugaTenshou);
        yield return new WaitForSeconds(tetsuStartUp);
        if (currentAnimeState != AnimeManMoveList.TetsugaTenshou)
        {
            uIManager.PHT("Interrupted!");
            swordBeam.SetActive(false);
            yield break;
        }
        canTakeDamage = false;
        uIManager.PHT("Sword Beam!");
        player.RecieveDamage(enemyStats.attackPower + tetsuDamage, enemyStats.impact, null, null);
        audioManager.PlayClip(AudioManager.ClipNames.Kick, AudioManager.ClipType.SFX);
        swordBeam.SetActive(false);
        yield return new WaitForSeconds(.2f);
        inAnotherState = false;
    }
    IEnumerator RoulletteCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);
        int roulletteNumber = 0;
        dice.SetActive(true);
        canTakeDamage = false;
        for (int i = 0; i < 20; i++)
        {
            roulletteNumber = UnityEngine.Random.Range(1, 20);
            dice.GetComponentInChildren<Text>().text = roulletteNumber.ToString();
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(1);
        DNDCalc(roulletteNumber);
        dice.SetActive(false);
        inAnotherState = false;
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
        if (powerChanged == true)
        {
            enemyStats.attackPower = attackOriginal;
            powerChanged = false;
        }
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        if (currState != EnemyStates.AttackingL || currState != EnemyStates.AttackingR)
            yield break;
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////

    void GeographyChangeF()
    {
        inFire = true;
        if (player.currentHealth - fireDamage <= 0)
            player.currentHealth -= 0;
        else
        {
            player.currentHealth -= fireDamage;
            uIManager.healthBar.value = player.currentHealth;
        }

        fireTicks++;
        if (fireTicks == timesToRepeat)
        {
            inFire = false;
            fireDamage = fDOriginal;
            CancelInvoke("GeographyChangeF");
        }
    }
    void GeographyChangeI()
    {
        inIce = true;
        FindObjectOfType<PlayerStats>().currentStamina -= iceDamage;
        uIManager.staminaBar.value = FindObjectOfType<PlayerStats>().currentStamina;
        iceTicks++;
        if (iceTicks == timesToRepeat)
        {
            inIce = false;
            iceDamage = iDOriginal;
            CancelInvoke("GeographyChangeI");
        }

    }

    void DNDCalc(int diceRoll)
    {
        float powerScale = 0;
        switch (diceRoll)
        {
            case 1:
                powerScale = .1f;
                break;
            case 2:
                powerScale = .2f;
                break;
            case 3:
                powerScale = .3f;
                break;
            case 4:
                powerScale = .4f;
                break;
            case 5:
                powerScale = .5f;
                break;
            case 6:
                powerScale = .6f;
                break;
            case 7:
                powerScale = .7f;
                break;
            case 8:
                powerScale = .8f;
                break;
            case 9:
                powerScale = .9f;
                break;
            case 10:
                powerScale = 1f;
                break;
            case 11:
                powerScale = 1.1f;
                break;
            case 12:
                powerScale = 1.2f;
                break;
            case 13:
                powerScale = 1.3f;
                break;
            case 14:
                powerScale = 1.4f;
                break;
            case 15:
                powerScale = 1.5f;
                break;
            case 16:
                powerScale = 1.6f;
                break;
            case 17:
                powerScale = 1.7f;
                break;
            case 18:
                powerScale = 1.8f;
                break;
            case 19:
                powerScale = 1.9f;
                break;
            case 20:
                powerScale = 2f;
                uIManager.PHT("Plus Ultimate!");
                break;
        }
        powerChanged = true;
        enemyStats.attackPower = (int)Mathf.Round(enemyStats.attackPower * powerScale);
    }
    void ChangeAnimeAnimationState(AnimeMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case AnimeMoveListAnim.RoomLeft:
                tempStateHolder = "RoomLeft";
                break;
            case AnimeMoveListAnim.RoomRight:
                tempStateHolder = "RoomRight";
                break;
            case AnimeMoveListAnim.RoomTakt:
                tempStateHolder = "RoomTakt";
                break;
            case AnimeMoveListAnim.Roullette:
                tempStateHolder = "Roullette";
                break;
            case AnimeMoveListAnim.TetsugaTenshou:
                tempStateHolder = "TetsugaTenshou";
                break;
            case AnimeMoveListAnim.Geography:
                tempStateHolder = "Geography";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentAnimeAnimState = newState;
    }
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(AnimeManMoveList.Nothing);
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
            swordBeam.SetActive(false);
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
            swordBeam.SetActive(false);
            //this is to make sure the movelist resets on hit
            StartMove(AnimeManMoveList.Nothing);
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
        swordBeam = GameObject.Find("SwordBeam");
        dice = GameObject.Find("Dice");
        swordBeam.SetActive(false);
        dice.SetActive(false);
    }
}
