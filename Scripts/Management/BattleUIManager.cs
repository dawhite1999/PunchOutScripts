using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public Text enemyStagger;
    public Text starPoints;
    public Text knockOutTimer;
    public Text roundTimer;
    public Text roundText;
    public Slider healthBar;
    public Slider staminaBar;
    public Slider enemyHealthBar;
    public Slider staggerBar;
    public Text dialogueTextP;
    public Text dialogueTextE;
    public GameObject coolDownBackground;
    public GameObject speechBubbleE;
    public GameObject speechBubbleP;
    public GameObject dangerousEnemy;
    List<GameObject> hypeTexts = new List<GameObject>();
    public bool doneTyping = false;
    public float currentRoundTime;
    public float maxRoundTime;
    public List<BaseCharacter> characters = new List<BaseCharacter>(); 
    // Start is called before the first frame update
    void Start()
    {
        enemyStagger = GameObject.Find("EnemyStagger").GetComponent<Text>();
        starPoints = GameObject.Find("StarPoints").GetComponent<Text>();
        knockOutTimer = GameObject.Find("KnockOutTimer").GetComponent<Text>();
        roundTimer = GameObject.Find("RoundTimer").GetComponent<Text>();
        roundText = GameObject.Find("RoundText").GetComponent<Text>();
        healthBar = GameObject.Find("Health").GetComponent<Slider>();
        staminaBar = GameObject.Find("Stamina").GetComponent<Slider>();
        enemyHealthBar = GameObject.Find("EnemyHealthBar").GetComponent<Slider>();
        staggerBar = GameObject.Find("StaggerBar").GetComponent<Slider>();
        dialogueTextE = GameObject.Find("EnemyDialougeText").GetComponent<Text>();
        dialogueTextP = GameObject.Find("PlayerDialougeText").GetComponent<Text>();
        coolDownBackground = GameObject.Find("CoolDownBackground");
        hypeTexts.Add(GameObject.Find("HypeText"));
        hypeTexts.Add(GameObject.Find("HypeText1"));
        hypeTexts.Add(GameObject.Find("HypeText2"));
        //Set text values
        enemyStagger.text = "";
        knockOutTimer.text = "";
        foreach (GameObject item in hypeTexts)
        {
            item.SetActive(false);
        }
        
        if (GameObject.Find("DangerousEnemy") != null)
        {
            dangerousEnemy = GameObject.Find("DangerousEnemy");
            dangerousEnemy.SetActive(false);
        }
        if (GameObject.Find("EnemySpeechBubble") != null)
        {
            speechBubbleE = GameObject.Find("EnemySpeechBubble");
            speechBubbleE.SetActive(false);
        }
        if (GameObject.Find("PlayerSpeechBubble") != null)
        {
            speechBubbleP = GameObject.Find("PlayerSpeechBubble");
            speechBubbleP.SetActive(false);
        }
        //set stats
        currentRoundTime = maxRoundTime;
        roundTimer.text = maxRoundTime.ToString();
        FindObjectOfType<BreakTime>().BreakTimeReferenceSet();
        FindObjectOfType<WinorLose>().SetWinorLoseScreen();
        FindObjectOfType<Player>().SetStatsandTexts();
        FindObjectOfType<Enemy>().SetEnemyStats();
        FindObjectOfType<Enemy>().SetSpecificEnemy();
        //find the characters
        //turn off the characters
        foreach (BaseCharacter foundCharacters in FindObjectsOfType<BaseCharacter>())
        {
            characters.Add(foundCharacters);
        }
        TurnOffCharacters();
    }
    public void TurnOffCharacters()
    {
        //turn off the characters
        foreach (BaseCharacter character in characters)
        {
            character.ToggleActive(false);
        }
    }
    public IEnumerator TypeTheSentence(string sentenceToTypeP, string sentenceToTypeE)
    {
        dialogueTextE.text = "";
        dialogueTextP.text = "";
        //type player sentence
        foreach (char letter in sentenceToTypeP.ToCharArray())
        {
            dialogueTextP.text += letter;        
            yield return new WaitForSeconds(.05f);
        }
        //type enemy sentence
        foreach (char letter in sentenceToTypeE.ToCharArray())
        {
            dialogueTextE.text += letter;
            yield return new WaitForSeconds(.05f);
        }
    }
    public void PHT(string incomingText)
    {
        StartCoroutine(PlayHypeText(incomingText));
    }
    IEnumerator PlayHypeText(string incomingText)
    {
        //go throught the list of hypetexts
        foreach (GameObject text in hypeTexts)
        {
            //if one of them is inactive
            if (text.activeSelf == false)
            {
                //turn it on, change the text, and feed it to the turn off function
                text.SetActive(true);
                text.GetComponent<TextMeshProUGUI>().text = incomingText;
                //play the animation with the corresponding index
                switch (hypeTexts.IndexOf(text))
                {
                    case 0:
                        //play anim 1
                        text.GetComponent<Animator>().Play("TextSlideIn");
                        break;
                    case 1:
                        //play anim 2
                        text.GetComponent<Animator>().Play("TextSlideIn2");
                        break;
                    case 2:
                        //play anim 3
                        text.GetComponent<Animator>().Play("TextSlideIn3");
                        break;
                }
                //wait for the animation to finish, turn text off
                yield return new WaitForSeconds(1.5f);
                text.SetActive(false);
                //leave the foreach loop
                break;
            }
        }
    }
}
