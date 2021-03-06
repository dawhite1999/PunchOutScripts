using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinorLose : MonoBehaviour
{
    GameObject winOrLoseScreen;
    Player player;
    Enemy enemy;
    BattleUIManager uIManager;
    Button returnToMain;
    Text victoryQuoteText;
    Text abilityUnlockText;
    public Text[] pointsEarnedTexts = new Text[3];
    public void SetWinorLoseScreen()
    {
        winOrLoseScreen = GameObject.Find("WinorLoseScreen");
        returnToMain = GameObject.Find("ReturnToMainButton").GetComponent<Button>();
        victoryQuoteText = GameObject.Find("EnemyDialougeTextWL").GetComponent<Text>();
        abilityUnlockText = GameObject.Find("AbilityUnlockText").GetComponent<Text>();
        returnToMain.onClick.AddListener(FindObjectOfType<SceneBoss>().EnemySelectScreen);
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        uIManager = GetComponent<BattleUIManager>();
        winOrLoseScreen.SetActive(false);
    }
    public void TurnonWinLose(string victoryQuote, bool playerWon)
    {
        StartCoroutine(StartEndScreen(victoryQuote, playerWon));
    }
    IEnumerator StartEndScreen(string victoryQuote, bool playerWin)
    {
        winOrLoseScreen.SetActive(true);
        uIManager.TurnOffCharacters();
        FindObjectOfType<AudioManager>().StopMusic();
        yield return new WaitForSeconds(1);
        //type player sentence
        foreach (char letter in victoryQuote.ToCharArray())
        {
            victoryQuoteText.text += letter;
            yield return new WaitForSeconds(.05f);
        }
        pointsEarnedTexts[0].text = "Gamer Points earned: " + enemy.pointsToGive[0];
        pointsEarnedTexts[1].text = "Meme Points earned: " + enemy.pointsToGive[1];
        pointsEarnedTexts[2].text = "Intelligence Points earned: " + enemy.pointsToGive[2];
        StaticStats.PGamerPoints += enemy.pointsToGive[0];
        StaticStats.PMemePoints += enemy.pointsToGive[1];
        StaticStats.PIntelPoints += enemy.pointsToGive[2];
        //unlock this enemy's ability
        if(playerWin == true)
        {
            switch(enemy.enemyName)
            {
                case "Pikay":
                    if (PlayerWins.pikayVictory == false)
                    {
                        PlayerWins.pikayVictory = true;
                        abilityUnlockText.text = "Pikay's Ability Unlocked!";
                        break;
                    }
                    else
                        break;
            }
        }
    }
}
