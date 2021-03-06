using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCharacter : MonoBehaviour
{
    //Variables
    public float maxHealth = 0;
    public float currentHealth;
    protected float netDamage;
    public bool canTakeDamage = true;
    public bool? checkOnce = false;
    protected bool countTime = true;
    protected bool canCount = false;
    protected bool stayDown = false;
    protected string tempStateHolder;

    public Animator animator;
    protected BattleUIManager uIManager;
    protected AudioManager audioManager;
    public string[] dialogueSentences;

    public virtual void RecieveDamage(float damageTaken, int impact, bool? lowHit, bool? starPunch)
    {
        currentHealth = currentHealth - damageTaken;
        Mathf.Round(currentHealth);
    }
    public void ToggleActive(bool turnOn)
    {
        if(turnOn == false)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
