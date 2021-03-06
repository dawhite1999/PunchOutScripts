using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySelection : MonoBehaviour
{
    //Unity stuff
    public GameObject[] gamerDisplays;
    public GameObject[] thinkersDisplays;
    public GameObject[] miscellaneousDisplays;
    public GameObject[] memelordDisplays;
    public GameObject[] allCharacterDisplays;
    GameObject[] tempLeague;
    Text enemyDesc;
    Text enemyLevel;
    Text enemyName;
    GameObject leftArrow;
    GameObject rightArrow;
    Button fightButton;

    //refs
    SceneBoss sceneBoss;
    
    //variables
    int selectedIndex;
   
    // Start is called before the first frame update
    void Start()
    {
        //set references
        leftArrow = GameObject.Find("LeftArrow");
        rightArrow = GameObject.Find("RightArrow");
        fightButton = GameObject.Find("FightButton").GetComponent<Button>();
        sceneBoss = FindObjectOfType<SceneBoss>();
        enemyDesc = GameObject.Find("Character Description").GetComponent<Text>();
        enemyLevel = GameObject.Find("Character Level").GetComponent<Text>();
        enemyName = GameObject.Find("Character Name").GetComponent<Text>();
        //set reference values
        //turn off all the displays
        foreach (GameObject leagueDisplay in allCharacterDisplays)
        {
            leagueDisplay.SetActive(false);
        }
        TurnOnArrows(false);
        enemyLevel.text = "";
        enemyDesc.text = "";
        enemyName.text = "";
        fightButton.interactable = false;
    }
    //Turn the default character on
    public void DisplayEnemy(string league)
    {
        selectedIndex = 0;
        foreach (GameObject leagueDisplay in allCharacterDisplays)
        {
            leagueDisplay.SetActive(false);
        }
        TurnOnArrows(true);
        switch (league)
        {
            case "Gamers":
                tempLeague = gamerDisplays;
                allCharacterDisplays[0].SetActive(true);
                foreach (GameObject characterDisplay in tempLeague)
                {
                    characterDisplay.SetActive(false);
                }
                gamerDisplays[selectedIndex].SetActive(true);        
                break;
            case "Thinkers":
                tempLeague = thinkersDisplays;
                allCharacterDisplays[1].SetActive(true);
                foreach (GameObject characterDisplay in tempLeague)
                {
                    characterDisplay.SetActive(false);
                }
                thinkersDisplays[selectedIndex].SetActive(true);
                break;
            case "Miscellaneous":
                tempLeague = miscellaneousDisplays;
                allCharacterDisplays[2].SetActive(true);
                foreach (GameObject characterDisplay in tempLeague)
                {
                    characterDisplay.SetActive(false);
                }
                miscellaneousDisplays[selectedIndex].SetActive(true);
                break;
            case "MemeLords":
                tempLeague = memelordDisplays;
                allCharacterDisplays[3].SetActive(true);
                foreach (GameObject characterDisplay in tempLeague)
                {
                    characterDisplay.SetActive(false);
                }
                memelordDisplays[selectedIndex].SetActive(true);
                break;
        }
        DisplayEnemyDesc(tempLeague);
    }
    //Scroll to the left or right
    public void EnemyScroll(bool isLeft)
    {
        if(isLeft == true)
            StartCoroutine(ScrollLeft());
        else
            StartCoroutine(ScrollRight());
    }
    void DisplayEnemyDesc(GameObject[] league)
    {
        enemyDesc.text = league[selectedIndex].GetComponent<EnemyDescription>().enemyDescription;
        enemyLevel.text = "Level " + league[selectedIndex].GetComponent<EnemyDescription>().enemyLevel;
        enemyName.text = league[selectedIndex].GetComponent<EnemyDescription>().enemyName;
        sceneBoss.PrepareBattle(tempLeague[selectedIndex].name);
        fightButton.interactable = true;
    }

    IEnumerator ScrollLeft()
    {   
        //if the index of the array has reached it's minimum, play animation, then skip to the max.
        if (selectedIndex == 0)
        {
            TurnOnArrows(false);
            //play anim
            gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ScrollLeft");
            yield return new WaitForSeconds(.33f);
            //turn off the current image
            tempLeague[selectedIndex].SetActive(false);
            //select new image and turn it on
            selectedIndex = tempLeague.Length - 1;
            tempLeague[selectedIndex].SetActive(true);
            //wait a bit to set idle and display description
            yield return new WaitForSeconds(.33f);
            gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ImageIdle");
            tempLeague[selectedIndex].GetComponent<Animator>().Play("PreviewIdle");
            DisplayEnemyDesc(tempLeague);
            TurnOnArrows(true);
            yield break;
        }
        TurnOnArrows(false);
        //play anim
        gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ScrollLeft");
        yield return new WaitForSeconds(.33f);
        //turn off the current image
        tempLeague[selectedIndex].SetActive(false);
        //select new image and turn it on
        selectedIndex--;
        tempLeague[selectedIndex].SetActive(true);
        //wait a bit to set idle and display description
        yield return new WaitForSeconds(.33f);
        gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ImageIdle");
        tempLeague[selectedIndex].GetComponent<Animator>().Play("PreviewIdle");
        DisplayEnemyDesc(tempLeague);
        TurnOnArrows(true);
    }
    IEnumerator ScrollRight()
    {
        //if the index of the array has reached it's minimum, play animation, then skip to the max.
        if (selectedIndex == gamerDisplays.Length -1)
        {
            TurnOnArrows(false);
            //play anim
            gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ScrollRight");
            yield return new WaitForSeconds(.33f);
            //turn off the current image
            tempLeague[selectedIndex].SetActive(false);
            //select new image and turn it on
            selectedIndex = 0;
            tempLeague[selectedIndex].SetActive(true);
            //wait a bit to set idle
            yield return new WaitForSeconds(.33f);
            gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ImageIdle");
            tempLeague[selectedIndex].GetComponent<Animator>().Play("PreviewIdle");
            DisplayEnemyDesc(tempLeague);
            TurnOnArrows(true);
            yield break;
        }
        TurnOnArrows(false);
        //play anim
        gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ScrollRight");
        yield return new WaitForSeconds(.33f);
        tempLeague[selectedIndex].SetActive(false);
        selectedIndex++;
        tempLeague[selectedIndex].SetActive(true);
        yield return new WaitForSeconds(.33f);
        gameObject.transform.Find("Characters").GetComponent<Animator>().Play("ImageIdle");
        tempLeague[selectedIndex].GetComponent<Animator>().Play("PreviewIdle");
        DisplayEnemyDesc(tempLeague);
        TurnOnArrows(true);
    }
    void TurnOnArrows(bool turnOn)
    {
        if(turnOn == true)
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
        else
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
    }
}
