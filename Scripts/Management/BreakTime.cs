using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTime : MonoBehaviour
{
    GameObject breakTimeScreen;
    Player player;
    Enemy enemy;
    PlayerStats playerStats;
    EnemyStats enemyStats;
    BattleUIManager uIManager;

    int rounds;
    bool checkOnce = false;

    public void BreakTimeReferenceSet()
    {
        breakTimeScreen = GameObject.Find("BreakTimeScreen");
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        playerStats = FindObjectOfType<PlayerStats>();
        enemyStats = FindObjectOfType<EnemyStats>();
        uIManager = GetComponent<BattleUIManager>();
    }
    // Update is called once per frame
    void Update()
    {
        //Check for input when break time is active
        if (Input.anyKeyDown && breakTimeScreen.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                return;
            if (rounds <= 1 && uIManager.dangerousEnemy != null && checkOnce == false)
                StartCoroutine(DECoroutine());

            uIManager.dialogueTextE.text = "";
            uIManager.dialogueTextP.text = "";
            breakTimeScreen.SetActive(false);
            //turn on the characters
            enemy.ToggleActive(true);
            player.ToggleActive(true);
            enemy.SetState(Enemy.EnemyStates.Idle);
            if(uIManager.dangerousEnemy != null)
                FindObjectOfType<AudioManager>().PlayClip(AudioManager.ClipNames.BossMusic, AudioManager.ClipType.Music);
            else
                FindObjectOfType<AudioManager>().PlayClip(AudioManager.ClipNames.BattleSong, AudioManager.ClipType.Music);
           // checkOnce = false;
        }
        //Count down time
        if (uIManager.currentRoundTime > 0 && breakTimeScreen.activeSelf == false && player != null && enemy != null)
        {
            if (player.currState == Player.PlayerStates.KnockDown || player.currState == Player.PlayerStates.TotalKnockOut)
            {
                //dont do anything
            }
            else if (enemy.currState == Enemy.EnemyStates.KnockedDown || enemy.currState == Enemy.EnemyStates.TotalKnockOut)
            {
                //dont do anything
            }
            else
            {
                uIManager.currentRoundTime -= Time.deltaTime;
                uIManager.roundTimer.text = uIManager.currentRoundTime.ToString("F0");
            }
        }
        else if(uIManager.currentRoundTime <= 0 && breakTimeScreen.activeSelf == false && checkOnce == false)
            StartCoroutine(BreakTimeCoroutine());
    }
    IEnumerator DECoroutine()
    {
        checkOnce = true;
        uIManager.dangerousEnemy.SetActive(true);
        player.disableInput = true;
        enemy.inAnotherState = true;
        yield return new WaitForSeconds(3);
        player.disableInput = false;
        enemy.inAnotherState = false;
        uIManager.dangerousEnemy.SetActive(false);
        checkOnce = false;
    }
    IEnumerator BreakTimeCoroutine()
    {
        checkOnce = true;
        //slow time
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
        //Refresh Stats
        uIManager.currentRoundTime = uIManager.maxRoundTime;
        uIManager.roundTimer.text = uIManager.currentRoundTime.ToString("F0");
        enemyStats.sameRoundKds = 0;
        playerStats.sameRoundKds = 0;
        //if in the seasauce battle
        if(FindObjectOfType<SeaSauce>() != null)
        {
            FindObjectOfType<SeaSauce>().startKOTimer = false;
            FindObjectOfType<SeaSauce>().koTimerText.SetActive(false);
        }
        FindObjectOfType<AudioManager>().StopMusic();
        //add rounds and subrtract knock down count
        rounds++;
        uIManager.roundText.text = "Round " + rounds;
        player.PlayerRefresh();
        enemy.EnemyRefresh();
        breakTimeScreen.SetActive(true);
        uIManager.TurnOffCharacters();
        //find something to say.
        int dialougePicker;
        dialougePicker = rounds;
        //if the rounds go on too long, pick a random thing to say.
        if (rounds > player.dialogueSentences.Length || rounds > enemy.dialogueSentences.Length)
            dialougePicker = Random.Range(1, 4);
        switch (dialougePicker)
        {
            case 1:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[0], enemy.dialogueSentences[0]));
                break;
            case 2:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[1], enemy.dialogueSentences[1]));
                break;
            case 3:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[2], enemy.dialogueSentences[2]));
                break;
            case 4:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[3], enemy.dialogueSentences[3]));
                break;
            case 5:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[4], enemy.dialogueSentences[4]));
                break;
            case 6:
                StartCoroutine(uIManager.TypeTheSentence(player.dialogueSentences[5], enemy.dialogueSentences[5]));
                break;
        }
        checkOnce = false;
    }
}
