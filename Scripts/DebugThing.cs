using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugThing : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StaticStats.PGamerPoints += 100;
            StaticStats.PIntelPoints += 100;
            StaticStats.PMemePoints += 100;
            FindObjectOfType<ArmorManager>().SetOwnedPointsTexts();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (string item in Perks.perkNamePick)
            {
                Debug.Log(item);
            }    
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindObjectOfType<PlayerStats>().attackPower = 1000;
            Debug.Log("Plus Ultra!!!");
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log(FindObjectOfType<Enemy>().currState);
            Debug.Log(FindObjectOfType<Player>().canTakeDamage);
        }
    }
}
