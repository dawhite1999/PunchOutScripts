using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    //Stats
    [Header("Enemy Stats")]
    public float attackPower;
    public int impact;
    public float punchWindUp;
    public float punchCoolDown = .5f;
    public float hitStun = .5f;
    public float comboFinisherHitStun= 1f;
    public float blockStun = .5f;
    [HideInInspector]
    public float currentIdleTime;
    public float maxIdleTimeCap;
    public float maxIdleTimeMin;
    [HideInInspector]
    public float currentComboTime;
    public float maxComboTime;
    [HideInInspector]
    public int currentComboHits;
    public int maxComboHits;
    [HideInInspector]
    public int currentStagger;
    public int maxStagger;
    public int staggerReduction;
    [HideInInspector]
    public int currentKDs;
    [HideInInspector]
    public int sameRoundKds;
    public int sameRoundMaxKds;
    public int maxKds;
    [HideInInspector]
    public float knockDownTime;
    public float maxKnockDownTime;
    [Header("Max Health is multiplied by this")]
    public float recoveredHealth;
    [Header("The lower this is, the dumber the ai")]
    public float repeatedLowHitTimeMax;
    [Header("The higher this is, the dumber the ai")]
    public int repeatedLowHitsMax;
    public float repeatedHighHitTimeMax;
    public int repeatedHighHitsMax;
    public float damageReduction;
}
