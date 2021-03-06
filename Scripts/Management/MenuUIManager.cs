using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    GameObject optionsScreen;
    bool opDisplay = false;
    // Start is called before the first frame update
    void Start()
    {
        optionsScreen = GameObject.Find("OptionsBackground");
        FindObjectOfType<AudioManager>().InitializeAudio();
        optionsScreen.SetActive(false);
    }
    public void DisplayOptions()
    {
        opDisplay = !opDisplay;
        if (opDisplay == true)
            optionsScreen.SetActive(true);
        else
            optionsScreen.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
