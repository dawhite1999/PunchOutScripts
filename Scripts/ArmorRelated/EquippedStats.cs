using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquippedStats : MonoBehaviour
{
    public List<GameObject> perkGO;
    public Dictionary<string, int> displayedPerkNames = new Dictionary<string, int>();
    public void AddPerkGOs()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            perkGO.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }
}
