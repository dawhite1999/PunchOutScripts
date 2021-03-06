using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMan : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject moveList;
    GameObject optionsScreen;
    GameObject breakTime;
    GameObject confirmLeave;
    Button resumeButton;
    Button moveListButton;
    Button optionsButton;
    Button eSButton;
    Button eMLButton;
    Button exitOptionsButton;
    Button yesButton;
    Button noButton;
    public bool willPause = false;
    bool moveListOn = false;
    bool opDisplay = false;
    bool confirmDisplayOn = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseBackground");
        moveList = GameObject.Find("MoveListBackground");
        optionsScreen = GameObject.Find("OptionsBackground");
        breakTime = GameObject.Find("BreakTimeScreen");
        confirmLeave = GameObject.Find("ConfirmEnemySelection");
        FindObjectOfType<AudioManager>().InitializeAudio();
        resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
        moveListButton = GameObject.Find("MoveListButton").GetComponent<Button>();
        optionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
        eSButton = GameObject.Find("EnemySelectionButton").GetComponent<Button>();
        eMLButton = GameObject.Find("QuitMoveListButton").GetComponent<Button>();
        exitOptionsButton = GameObject.Find("ExitOptions").GetComponent<Button>();
        yesButton = GameObject.Find("YesButton").GetComponent<Button>();
        noButton = GameObject.Find("NoButton").GetComponent<Button>();
        resumeButton.onClick.AddListener(Pause);
        moveListButton.onClick.AddListener(SummonMoveList);
        optionsButton.onClick.AddListener(DisplayOptions);
        eSButton.onClick.AddListener(ConfirmLeave);
        eMLButton.onClick.AddListener(SummonMoveList);
        exitOptionsButton.onClick.AddListener(DisplayOptions);
        yesButton.onClick.AddListener(FindObjectOfType<SceneBoss>().EnemySelectScreen);
        noButton.onClick.AddListener(ConfirmLeave);
        optionsScreen.SetActive(false);
        pauseMenu.SetActive(false);
        moveList.SetActive(false);
        confirmLeave.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && breakTime.activeSelf == false)
            Pause();
    }
    public void Pause()
    {
        willPause = !willPause;
        if (willPause == true)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }       
        else
        {
            Time.timeScale = 1;
            if (moveListOn == true)
                SummonMoveList();
            if (opDisplay == true)
                DisplayOptions();
            if (confirmDisplayOn == true)
                ConfirmLeave();
            pauseMenu.SetActive(false);
        }
    }
    public void SummonMoveList()
    {
        moveListOn = !moveListOn;
        if (moveListOn == true)
            moveList.SetActive(true);
        else
            moveList.SetActive(false);
    }
    public void DisplayOptions()
    {
        opDisplay = !opDisplay;
        if (opDisplay == true)
            optionsScreen.SetActive(true);
        else
            optionsScreen.SetActive(false);
    }
    void ConfirmLeave()
    {
        confirmDisplayOn = !confirmDisplayOn;
        if (confirmDisplayOn == true)
            confirmLeave.SetActive(true);
        else
            confirmLeave.SetActive(false);
    }
}
