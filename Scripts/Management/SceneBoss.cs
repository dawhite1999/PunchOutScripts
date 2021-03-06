using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBoss : MonoBehaviour
{
    string battleToLoad;
    GameObject fadeScreen;
    private void Start()
    {
        fadeScreen = GameObject.Find("FadeScreen");
        StartCoroutine(ScreenOpenCoroutine());
    }
    //called by buttons
    public void EnemySelectScreen()
    {
        StartCoroutine(EnemySelectScreenCoroutine());
    }
    public void LoadBattle()
    {
        StartCoroutine(LoadBattleCoroutine());
    }
    public void PrepareBattle(string battle)
    {
        battleToLoad = battle + "Battle";
    }
    public void MainMenuLoad()
    {
        StartCoroutine(LoadMainMenuCoroutine());
    }
    public void ArmorSceneLoad()
    {
        StartCoroutine(LoadArmor());
    }
    //coroutines
    IEnumerator ScreenOpenCoroutine()
    {
        fadeScreen.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(.499f);
        fadeScreen.SetActive(false);
    }
    IEnumerator LoadArmor()
    {
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Animator>().SetBool("fadeInOn", true);
        yield return new WaitForSeconds(.45f);
        SceneManager.LoadScene("ArmorSelection");
    }
    IEnumerator EnemySelectScreenCoroutine()
    {
        Time.timeScale = 1;
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Animator>().SetBool("fadeInOn", true);
        yield return new WaitForSeconds(.45f);
        SceneManager.LoadScene("EnemySelectScene");
    }
    IEnumerator LoadBattleCoroutine()
    {
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Animator>().Play("FadeIn");
        yield return new WaitForSeconds(.45f);
        SceneManager.LoadScene(battleToLoad);
    }
    IEnumerator LoadMainMenuCoroutine()
    {
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Animator>().Play("FadeIn");
        yield return new WaitForSeconds(.45f);
        SceneManager.LoadScene("Main Menu");
    }
}
