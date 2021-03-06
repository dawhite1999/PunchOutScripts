using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InventoryArmor : MonoBehaviour
{
    public List<GameObject> craftedArmor;
    int storedArmor = 0;
    //The false version is called by a button, the true is called on startup by armormanager
    public void BuildInv(bool isFirst)
    {
        int armorCounter = 0;
        if(isFirst == true)
        {
            foreach (GameObject armor in craftedArmor)
            {
                //count the armor
                storedArmor++;
                Instantiate(armor, transform);
            }
        }
        else
        {
            //dont instantiate already instansiated armor
            foreach (GameObject armor in craftedArmor)
            {
                armorCounter++;
                if(armorCounter > storedArmor)
                {
                    storedArmor++;
                    Instantiate(armor, transform);
                }
                armor.SetActive(true);
            }
        }
    }
}
