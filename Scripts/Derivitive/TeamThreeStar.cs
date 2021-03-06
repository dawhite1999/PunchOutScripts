using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TeamThreeStar : Enemy
{
    Dictionary<TTSMoveList, Action> TTSDict = new Dictionary<TTSMoveList, Action>();
    TTSMoveListAnim currentAnimState;
    TTSMoveList currentTTSState;
    GameObject solarFlareScreen;
    GameObject sFCursedImage;
    GameObject kaioKenAura;
    GameObject kamehamehaBeam;
    public GameObject playerOwnedCount;
    public GameObject[] muffinImages = new GameObject[3];
    //Variables
    [SerializeField]
    float kamehamehaStartUp;
    [SerializeField]
    float muffinStartUp;
    [SerializeField]
    float solarStartUp;
    [SerializeField]
    float kamehamehaDamage;
    [SerializeField]
    int kamehamehaImpact;
    [SerializeField]
    int sensuBeans;
    [SerializeField]
    float kaioKenDurationMax;
    float currKaioKenDuration;
    int muffins = 0;
    bool kaioKenActive = false;
    int kaioKenLevel = 0;
    public enum TTSMoveList
    {
        KaioKen,
        Kamehameha,
        SensuBean,
        SolarFlare,
        MuffinButton,
        MrPoPo,
        Nothing,
    }
    public enum TTSMoveListAnim
    {
        KaioKen1,
        KaioKen2,
        KaioKen3,
        Kamehameha,
        SensuBean,
        SolarFlare,
        MuffinButton,
        MrPoPo,
    }
    public override void Start()
    {
        base.Start();
        TTSDict.Add(TTSMoveList.KaioKen, new Action(KaioKenOn));
        TTSDict.Add(TTSMoveList.Kamehameha, new Action(KamehamehaOn));
        TTSDict.Add(TTSMoveList.SensuBean, new Action(SensuBeanOn));
        TTSDict.Add(TTSMoveList.SolarFlare, new Action(SolarFlareOn));
        TTSDict.Add(TTSMoveList.MuffinButton, new Action(MuffinButtonOn));
        TTSDict.Add(TTSMoveList.MrPoPo, new Action(MrPoPoOn));
        TTSDict.Add(TTSMoveList.Nothing, new Action(Nothing));
        currKaioKenDuration = kaioKenDurationMax;
    }
    void StartMove(TTSMoveList newState)
    {
        inAnotherState = true;
        currentTTSState = newState;
        TTSDict[currentTTSState].Invoke();
    }
    protected override void Update()
    {
        base.Update();
        if (kaioKenActive == true)
        {
            currKaioKenDuration -= Time.deltaTime;
            if (currKaioKenDuration <= 0)
                KaioKenCoolOff(kaioKenLevel, false);
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
        if (attackPicker < 40)
            SetState(EnemyStates.AttackingL);
        else if (attackPicker >= 40 && attackPicker < 50)
            StartMove(TTSMoveList.Kamehameha);
        else if (attackPicker >= 50 && attackPicker < 60)
            StartMove(TTSMoveList.SolarFlare);
        else if (attackPicker >= 60 && attackPicker < 75)
            StartMove(TTSMoveList.MuffinButton);
        else if (attackPicker >= 75 && attackPicker < 80 && currentHealth < currentHealth / 2)
        {
            if (sensuBeans > 0)
                StartMove(TTSMoveList.SensuBean);
            else
                SetState(EnemyStates.AttackingR);
        }
        else if (attackPicker >= 80 && attackPicker < 90)
        {
            if(kaioKenActive == false)
                StartMove(TTSMoveList.KaioKen);
            else
                SetState(EnemyStates.AttackingR);
        }
        else if (attackPicker >= 90 && attackPicker <= 100)
            StartMove(TTSMoveList.MrPoPo);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////
    void KaioKenOn()
    {
        StartCoroutine(KaioKenCoroutine());
    }
    void KamehamehaOn()
    {
        StartCoroutine(KamehamehaCoroutine());
    }
    void SensuBeanOn()
    {
        StartCoroutine(SensuBeanCoroutine());
    }
    void SolarFlareOn()
    {
        StartCoroutine(SolarFlareCoroutine());
    }
    void MuffinButtonOn()
    {
        StartCoroutine(MuffinButtonCoroutine());
    }
    void MrPoPoOn()
    {
        StartCoroutine(MrPoPoCoroutine());
    }
    void Nothing()
    {
        inAnotherState = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////States//////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator KaioKenCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        kaioKenAura.SetActive(true);
        canTakeDamage = false;
        uIManager.speechBubbleE.SetActive(true);
        uIManager.speechBubbleP.SetActive(true);
        if (currentHealth >= maxHealth * .8)
            kaioKenLevel = 2;
        else if (currentHealth >= maxHealth * .5f)
            kaioKenLevel = 4;
        else if (currentHealth < maxHealth * .5f)
            kaioKenLevel = 20;
        switch(kaioKenLevel)
        {
            case 2:
                ChangeTTSAnimationState(TTSMoveListAnim.KaioKen1);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "Heavenly Fist!";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.SetActive(false);
                uIManager.speechBubbleP.SetActive(false);
                uIManager.PHT("Sparking!");
                break;
            case 4:
                ChangeTTSAnimationState(TTSMoveListAnim.KaioKen2);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "Heavenly Fist...";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "Times four!";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.SetActive(false);
                uIManager.speechBubbleP.SetActive(false);
                uIManager.PHT("Sparking!!");
                break;
            case 20:
                ChangeTTSAnimationState(TTSMoveListAnim.KaioKen3);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "Heavenly Fist...";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "Times...";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.GetComponentInChildren<Text>().text = "20!!!";
                yield return new WaitForSeconds(1);
                uIManager.speechBubbleE.SetActive(false);
                uIManager.speechBubbleP.SetActive(false);
                uIManager.PHT("Sparking!!!");
                break;
        }
        KaioKenCalcs(kaioKenLevel);
        yield return new WaitForSeconds(.5f);
        inAnotherState = false;
        SetState(EnemyStates.Idle);
    }
    IEnumerator KamehamehaCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        kamehamehaBeam.SetActive(true);
        ChangeTTSAnimationState(TTSMoveListAnim.Kamehameha);
        yield return new WaitForSeconds(kamehamehaStartUp);
        if (currentTTSState != TTSMoveList.Kamehameha)
        {
            uIManager.PHT("Interrupted!");
            kamehamehaBeam.SetActive(false);
            yield break;
        }
        canTakeDamage = false;
        uIManager.PHT("EnergyBeam!");
        player.RecieveDamage(kamehamehaDamage, kamehamehaImpact, null, null);
        audioManager.PlayClip(AudioManager.ClipNames.Kick, AudioManager.ClipType.SFX);
        kamehamehaBeam.SetActive(false);
        yield return new WaitForSeconds(enemyStats.punchCoolDown);
        inAnotherState = false;
    }
    IEnumerator SensuBeanCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        canTakeDamage = false;
        sensuBeans--;
        ChangeTTSAnimationState(TTSMoveListAnim.SensuBean);
        uIManager.PHT("Bean Daddy!");
        yield return new WaitForSeconds(1);
        //refresh stats
        currentHealth = maxHealth;
        enemyStats.sameRoundKds = 0;
        enemyStats.currentStagger = 0;
        //set ui
        uIManager.enemyHealthBar.value = currentHealth;
        uIManager.staggerBar.value = enemyStats.currentStagger;
        inAnotherState = false;
    }
    IEnumerator SolarFlareCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        ChangeTTSAnimationState(TTSMoveListAnim.SolarFlare);
        yield return new WaitForSeconds(solarStartUp);
        if (currentTTSState != TTSMoveList.SolarFlare)
        {
            Debug.Log(currentTTSState);
            uIManager.PHT("Interrupted!");
            inAnotherState = false;
            yield break;
        }
        if(player.currState != Player.PlayerStates.Blocking)
        {
            canTakeDamage = false;
            uIManager.PHT("SunFlash!");
            solarFlareScreen.SetActive(true);
            yield return new WaitForSeconds(1);
            sFCursedImage.SetActive(true);
            yield return new WaitForSeconds(.1f);
            sFCursedImage.SetActive(false);
            yield return new WaitForSeconds(2.9f);
            solarFlareScreen.SetActive(false);
        }
        else
            uIManager.PHT("SunFlash Missed!");
        inAnotherState = false;
    }
    IEnumerator MuffinButtonCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        ChangeTTSAnimationState(TTSMoveListAnim.MuffinButton);
        yield return new WaitForSeconds(muffinStartUp);
        if(currentTTSState != TTSMoveList.MuffinButton)
        {
            uIManager.PHT("Interrupted!");
            yield break;
        }
        muffins = 3;
        foreach (GameObject muffin in muffinImages)
        {
            muffin.SetActive(true);
        }
        inAnotherState = false;
    }
    IEnumerator MrPoPoCoroutine()
    {
        //make the state machine in enemy script do nothing while it waits for this move to be finished
        inAnotherState = true;
        SetState(EnemyStates.Idle);

        canTakeDamage = false;
        ChangeTTSAnimationState(TTSMoveListAnim.MrPoPo);
        yield return new WaitForSeconds(2);
        player.RecieveDamage(100000, 1, null, null);
        inAnotherState = false;
    }
    protected override IEnumerator TauntCoroutine()
    {
        checkOnce = true;
        canTakeDamage = false;
        ChangeAnimationState(AnimationStates.TAUNT);
        yield return new WaitForSeconds(2);
        canTakeDamage = true;
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////Enumerators//////////////////////////////////////////////////////////////////////////////////////////////////////////

    void KaioKenCalcs(int kaioLevel)
    {
        //set stats
        kaioKenActive = true;
        enemyStats.attackPower = enemyStats.attackPower * kaioLevel;
        enemyStats.impact = enemyStats.impact * kaioLevel;
        kamehamehaDamage = kamehamehaDamage * kaioLevel;
        kamehamehaImpact = kamehamehaImpact * kaioLevel;
        enemyStats.maxStagger = enemyStats.maxStagger * kaioLevel;
        enemyStats.staggerReduction = enemyStats.staggerReduction * kaioLevel;
        maxHealth = maxHealth * kaioLevel;
        currentHealth = currentHealth * kaioLevel;
        switch (kaioLevel)
        {
            case 2:
                enemyStats.punchWindUp = enemyStats.punchWindUp - 0.1f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap--;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin--;
                enemyStats.maxComboTime = enemyStats.maxComboTime--;
                break;
            case 4:
                enemyStats.punchWindUp = enemyStats.punchWindUp - 0.2f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap - 2;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin - 2;
                enemyStats.maxComboTime = enemyStats.maxComboTime - 2;
                break;
            case 20:
                enemyStats.punchWindUp = enemyStats.punchWindUp - 0.4f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap - 4;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin - 4;
                enemyStats.maxComboTime = enemyStats.maxComboTime - 4;
                break;
        }
        if (enemyStats.punchWindUp < 0)
            enemyStats.punchWindUp = 0;
        if (enemyStats.maxIdleTimeCap < 0)
            enemyStats.maxIdleTimeCap = 0;
        if (enemyStats.maxIdleTimeMin < 0)
            enemyStats.maxIdleTimeMin = 0;
        if (enemyStats.maxComboTime < 0)
            enemyStats.maxComboTime = 0;
        //set ui
        uIManager.enemyHealthBar.value = currentHealth;
        uIManager.staggerBar.value = enemyStats.currentStagger;
    }
    void KaioKenCoolOff(int kaioLevel, bool wasKOed)
    {
        if (kaioLevel == 0)
            return;
        //set stats
        kaioKenActive = false;
        kaioKenAura.SetActive(false);
        currKaioKenDuration = kaioKenDurationMax;
        enemyStats.attackPower = enemyStats.attackPower / kaioLevel;
        enemyStats.impact = enemyStats.impact / kaioLevel;
        kamehamehaDamage = kamehamehaDamage / kaioLevel;
        kamehamehaImpact = kamehamehaImpact / kaioLevel;
        enemyStats.maxStagger = enemyStats.maxStagger / kaioLevel;
        enemyStats.staggerReduction = enemyStats.staggerReduction / kaioLevel;
        maxHealth = maxHealth / kaioLevel;
        currentHealth = currentHealth / kaioLevel;
        switch (kaioLevel)
        {
            case 2:
                enemyStats.punchWindUp = enemyStats.punchWindUp + 0.1f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap++;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin++;
                enemyStats.maxComboTime = enemyStats.maxComboTime++;
                break;
            case 4:
                enemyStats.punchWindUp = enemyStats.punchWindUp + 0.2f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap + 2;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin + 2;
                enemyStats.maxComboTime = enemyStats.maxComboTime + 2;
                currentHealth = currentHealth * .8f;
                break;
            case 20:
                enemyStats.punchWindUp = enemyStats.punchWindUp + 0.4f;
                enemyStats.maxIdleTimeCap = enemyStats.maxIdleTimeCap + 4;
                enemyStats.maxIdleTimeMin = enemyStats.maxIdleTimeMin + 4;
                enemyStats.maxComboTime = enemyStats.maxComboTime + 4;
                currentHealth = currentHealth * .4f;
                if (currentHealth <= 0 && wasKOed == true)
                    break;
                //set ui
                uIManager.enemyHealthBar.value = currentHealth;
                uIManager.staggerBar.value = enemyStats.currentStagger;
                isStaggered = true;
                uIManager.PHT("Stagger!");
                uIManager.enemyStagger.text = "Staggered!";
                //set state
                SetState(EnemyStates.Staggered);
                uIManager.enemyStagger.GetComponent<Animator>().Play("StaggerTextPop");
                audioManager.PlayClip(AudioManager.ClipNames.Stagger, AudioManager.ClipType.SFX);
                //Play stagger anim
                ChangeAnimationState(AnimationStates.STAGGERED);
                break;
        }
        if (currentHealth <= 0 && wasKOed == false)
            currentHealth = 1;
        inAnotherState = false;
    }
    public override void RecieveDamage(float damageTaken, int staggerAdditive, bool? lowHit, bool? starPunch)
    {
        hitLow = lowHit;
        //When staggered
        if (enemyStats.currentComboHits < enemyStats.maxComboHits && isStaggered == true)
        {
            //this is to make sure the movelist resets on hit
            StartMove(TTSMoveList.Nothing);
            //the line below is here to prevent the functionality that happens when you meet the stagger threshold from repeating everytime the enemy is hit in the staggered state
            enemyStats.currentStagger += staggerAdditive;
            enemyStats.currentComboHits++;
            //Set State
            SetState(EnemyStates.HitStun);
        }
        //For the muffin ability
        if(muffins > 0)
        {
            muffins--;
            uIManager.PHT("Muffin Block!");
            muffinImages[muffins].SetActive(false);
            return;
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
            kamehamehaBeam.SetActive(false);
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
            kamehamehaBeam.SetActive(false);
            //this is to make sure the movelist resets on hit
            StartMove(TTSMoveList.Nothing);
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
            KaioKenCoolOff(kaioKenLevel, true);
            uIManager.enemyHealthBar.value = currentHealth;
            enemyStats.currentKDs++;
            enemyStats.sameRoundKds++;
            RefreshStaggerVars();
            uIManager.PHT("Knock Out!");
            SetState(EnemyStates.KnockedDown);
        }
    }

    void ChangeTTSAnimationState(TTSMoveListAnim newState)
    {
        //stop the same animation from interrupting itself
        switch (newState)
        {
            case TTSMoveListAnim.KaioKen1:
                tempStateHolder = "KaioKen1";
                break;
            case TTSMoveListAnim.KaioKen2:
                tempStateHolder = "KaioKen2";
                break;
            case TTSMoveListAnim.KaioKen3:
                tempStateHolder = "KaioKen3";
                break;
            case TTSMoveListAnim.Kamehameha:
                tempStateHolder = "Kamehameha";
                break;
            case TTSMoveListAnim.MrPoPo:
                tempStateHolder = "MrPoPo";
                break;
            case TTSMoveListAnim.MuffinButton:
                tempStateHolder = "MuffinButton";
                break;
            case TTSMoveListAnim.SensuBean:
                tempStateHolder = "SensuBean";
                break;
            case TTSMoveListAnim.SolarFlare:
                tempStateHolder = "SolarFlare";
                break;
        }

        //Play animation
        animator.Play(tempStateHolder);
        currentAnimState = newState;
    }

    public override void SetSpecificEnemy()
    {
        base.SetSpecificEnemy();
        solarFlareScreen = GameObject.Find("SolarFlareScreen");
        sFCursedImage = GameObject.Find("SFCursedImage");
        kaioKenAura = GameObject.Find("KaioKenAura");
        kamehamehaBeam = GameObject.Find("KamehamehaBeam");
        playerOwnedCount = GameObject.Find("POwnedCounter");
        playerOwnedCount.GetComponentInChildren<Text>().text = "Player owned count: 0";
        playerOwnedCount.SetActive(false);
        solarFlareScreen.SetActive(false);
        sFCursedImage.SetActive(false);
        kaioKenAura.SetActive(false);
        kamehamehaBeam.SetActive(false);
        foreach (GameObject muffin in muffinImages)
        {
            muffin.SetActive(false);
        }
    }
}
