using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Stats
    [Header("Player Stats")]
    public int attackPower;
    public int criticalChance;
    public float criticalDamage;
    [HideInInspector]
    public float currentStamina;
    public int maxStamina;
    [HideInInspector]
    public float speed;
    public int impact;
    [HideInInspector]
    public float currentExhaustionTime;
    public float maxExhaustionTime;
    public int starPoints;
    public int starPunchDamage;
    public float dodgeStartUp;
    [HideInInspector]
    public float knockDownTime;
    public float maxKnockDownTime;
    [HideInInspector]
    public int currentDownedInputs;
    public int maxDownedInputs;
    [Header("Health is multiplied by this")]
    public float recoveredHealth;
    [HideInInspector]
    public int currentKds;
    public int maxKds;
    [HideInInspector]
    public int sameRoundKds;
    public int sameRoundMaxKds;
    public float damageReduction;
    public float starPunchStartUp;
    public float punchStartUp;
    public float punchCoolDown = .2f;
    public float blockStun = .5f;
    public float hitStun = .5f;
    public float dodgeDownInvulDuration = .3f;
    public float dodgeInvulDuration = .7f;

    private void Awake()
    {
        ApplyPerks();
    }
    private void Start()
    {
        starPunchDamage = attackPower * 2;
    }
    public void ApplyPerks()
    {
        foreach (string perkName in Perks.perkNamePick)
        {
            ConvertPerktoStat(perkName);
        }
    }
    void ConvertPerktoStat(string perk)
    {
        switch(perk.ToLower())
        {
            case "attack boost":
               attackPower = (int)CalculateStat(Perks.PerkDictionary[perk.ToLower()], 1.4f, 0.3f, attackPower);
                break;
            case "critcal boost":
               criticalChance = (int)CalculateStat(Perks.PerkDictionary[perk.ToLower()], 1.4f, 0.7f, criticalChance);
                break;
            case "critical damage boost":
                criticalDamage = CalculateStat(Perks.PerkDictionary[perk.ToLower()], 1.1f, 0.9f, criticalDamage);
                break;
            case "health boost":
                GetComponent<Player>().maxHealth = CalculateStat(Perks.PerkDictionary[perk.ToLower()], 1.2f, 0.5f, GetComponent<Player>().maxHealth);
                break;
        }
    }
    float CalculateStat(int perkLevel, float statMultiplier, float statAdditive, float stat)
    {
        //do balancing here
        for (int i = 0; i < perkLevel; i++)
        {
            stat = Mathf.Round((stat * statMultiplier) + statAdditive);
        }
        return stat;
    }
}
