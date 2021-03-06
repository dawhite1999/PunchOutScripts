using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Perks
{
    public static Dictionary<string, int> PerkDictionary = new Dictionary<string, int>();
    public static List<string> perkNamePick = new List<string>();
    static bool initialized;
    public static void InitializePerks()
    {
        if(initialized == false)
        {
            PerkDictionary.Add("attack boost", 0);
            PerkDictionary.Add("critcal boost", 0);
            PerkDictionary.Add("critical damage boost", 0);
            PerkDictionary.Add("health boost", 0);
            initialized = true;
        }
    }
}
