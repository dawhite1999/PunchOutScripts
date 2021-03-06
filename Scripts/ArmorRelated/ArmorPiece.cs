using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmorPiece : MonoBehaviour
{
    //vars
    public bool? isBonus;
    [Header("these have to be the same as the keys in the dict. in the perks class (add cap letters)")]
    public string[] perksNOnThis;
    public List<int> perksVOnThis;
    int[] perksVOnThisCopy;
    [Header("this has to be the same as one of the perks on this armor piece")]
    public string BonusStatName;
    public int BonusStatValue;
    [SerializeField]
    string armorType;
    string tempNameTag;
    EquippedStats equippedStats;
    ArmorManager armorManager;

    //unity
    private void Start()
    {
        //copy the original list
        perksVOnThisCopy = perksVOnThis.ToArray();
        armorManager = FindObjectOfType<ArmorManager>();
        equippedStats = FindObjectOfType<EquippedStats>();
    }
    public void EquipArmor(bool equipping)
    {
        if(equipping == true)
        {
            foreach (string perkName in perksNOnThis)
            {
                //change the perk value in the dictionary
                Perks.PerkDictionary[perkName.ToLower()] += perksVOnThis[0];
                //remove the first perk value in the list so the next perk value can be set correctly.
                perksVOnThis.RemoveAt(0);
            }
            //add or subtract the bonus perk
            if(isBonus == false)
                Perks.PerkDictionary[BonusStatName.ToLower()] -= BonusStatValue;
            else if(isBonus == true)
                Perks.PerkDictionary[BonusStatName.ToLower()] += BonusStatValue;
            GetComponent<Button>().interactable = false;
            DisplayEquippedStats(true);
        }
        else
        {
            foreach (string perkName in perksNOnThis)
            {
                //change the perk value in the dictionary
                Perks.PerkDictionary[perkName.ToLower()] -= perksVOnThis[0];
                //remove the first perk value in the list so the next perk value can be set correctly.
                perksVOnThis.RemoveAt(0);
            }
            DisplayEquippedStats(false);
            return;
        }
        //reset the list
        perksVOnThis.Add(perksVOnThisCopy[0]);
        perksVOnThis.Add(perksVOnThisCopy[1]);
        //go through the array on player equip, and add this armor piece to the correct spot in the array.
        switch(armorType.ToLower())
        {
            case "helmet":
                GiveArmorToArmorSet(0);
                break;
            case "chestplate":
                GiveArmorToArmorSet(1);
                break;
            case "gloves":
                GiveArmorToArmorSet(2);
                break;
            case "pants":
                GiveArmorToArmorSet(3);
                break;
            case "boots":
                GiveArmorToArmorSet(4);
                break;
        }
    }
    //This stuff is so bonkers for something so simple, get ready for a ride
    void DisplayEquippedStats(bool equipping)
    {
        if(equipping == true)
        {
            //if there is something in the list of perks being displayed, do this
            if (equippedStats.displayedPerkNames.Count > 0)
            {
                //we need to repeat code for the two different perks
                //if the name of the first perk exists in our name dictionary
                if (equippedStats.displayedPerkNames.ContainsKey(perksNOnThis[0]))
                {
                    //add one to the number associated with that perk name in the dictionary
                    equippedStats.displayedPerkNames[perksNOnThis[0]]++;
                    //find the text with the matching name
                    foreach (GameObject perkText in equippedStats.perkGO)
                    {
                        if (perkText.GetComponent<PerkTag>().perkNameTag == perksNOnThis[0])
                        {
                            //just rename the already existing one, with the new value
                            perkText.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[0].ToLower()];
                            perkText.GetComponent<PerkTag>().ShowPerk();
                        }
                    }
                }
                else
                    SpawnNewPerkText(2);
                //if the name of the second perk exists in our name list
                if (equippedStats.displayedPerkNames.ContainsKey(perksNOnThis[1]))
                {
                    //add one to the number associated with that perk name in the dictionary
                    equippedStats.displayedPerkNames[perksNOnThis[1]]++;
                    //find the text with the matching name
                    foreach (GameObject perkText in equippedStats.perkGO)
                    {
                        if (perkText.GetComponent<PerkTag>().perkNameTag == perksNOnThis[1])
                        {
                            //just rename the already existing one, with the new value
                            perkText.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[0].ToLower()];
                            perkText.GetComponent<PerkTag>().ShowPerk();
                        }
                    }
                }
                else
                    SpawnNewPerkText(1);
            }
            else
                SpawnNewPerkText(4);
        }
        else
        {
            //If the number of "names" of the perk we are removing is greater than 1
            if(equippedStats.displayedPerkNames[perksNOnThis[0]] > 1)
            {
                //subtract one from the number associated with that perk name in the dictionary
                equippedStats.displayedPerkNames[perksNOnThis[0]]--;
                //find the text with the matching name
                foreach (GameObject perkText in equippedStats.perkGO)
                {
                    if (perkText.GetComponent<PerkTag>().perkNameTag == perksNOnThis[0])
                    {
                        //just rename the already existing one, with the new value
                        perkText.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[0].ToLower()];
                        perkText.GetComponent<PerkTag>().ShowPerk();
                    }
                }
            }
            else
                ClearPerkText(2);
            //If the number of "names" of the perk we are removing is greater than 1
            if (equippedStats.displayedPerkNames[perksNOnThis[1]] > 1)
            {
                //subtract one from the number associated with that perk name in the dictionary
                equippedStats.displayedPerkNames[perksNOnThis[1]]--;
                //find the text with the matching name
                foreach (GameObject perkText in equippedStats.perkGO)
                {
                    if (perkText.GetComponent<PerkTag>().perkNameTag == perksNOnThis[0])
                    {
                        //just rename the already existing one, with the new value
                        perkText.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[0].ToLower()];
                        perkText.GetComponent<PerkTag>().ShowPerk();
                    }
                }
            }
            else
                ClearPerkText(1);
        }
    }
    void SpawnNewPerkText(int instructions)
    {
        if(instructions % 2 == 0)
        {
            //set values in empty spaces
            foreach (GameObject item in equippedStats.perkGO)
            {
                if(item.GetComponent<PerkTag>().perkNameTag == "")
                {
                    item.GetComponent<PerkTag>().perkNameTag = perksNOnThis[0];
                    item.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[0].ToLower().ToString()];
                    item.GetComponent<PerkTag>().ShowPerk();
                    //add the name to the name dictionary and to the static list
                    equippedStats.displayedPerkNames.Add(item.GetComponent<PerkTag>().perkNameTag, 1);
                    Perks.perkNamePick.Add(item.GetComponent<PerkTag>().perkNameTag);
                    break;
                }
            }     
            if (instructions == 2)
                return;
        }
        //make second text clone, rename it, add it to the list
        //set values in the original text
        foreach (GameObject item in equippedStats.perkGO)
        {
            if (item.GetComponent<PerkTag>().perkNameTag == "")
            {
                item.GetComponent<PerkTag>().perkNameTag = perksNOnThis[1];
                item.GetComponent<PerkTag>().perkValue = Perks.PerkDictionary[perksNOnThis[1].ToLower().ToString()];
                item.GetComponent<PerkTag>().ShowPerk();
                //add the name to the name dictionary and the static list
                equippedStats.displayedPerkNames.Add(item.GetComponent<PerkTag>().perkNameTag, 1);
                Perks.perkNamePick.Add(item.GetComponent<PerkTag>().perkNameTag);
                break;
            }
        }    
    }
    void ClearPerkText(int instructions)
    {
        if (instructions % 2 == 0)
        {
            foreach (GameObject item in equippedStats.perkGO)
            {
                if (item.GetComponent<PerkTag>().perkNameTag == perksNOnThis[0])
                {
                    item.GetComponent<PerkTag>().ClearPerk();
                    //remove the name from the name dictionary
                    equippedStats.displayedPerkNames.Remove(perksNOnThis[0]);
                    Perks.perkNamePick.Remove(perksNOnThis[0]);
                    break;
                }
            }
            if (instructions == 2)
                return;
        }
        foreach (GameObject item in equippedStats.perkGO)
        {
            if (item.GetComponent<PerkTag>().perkNameTag == perksNOnThis[1])
            {
                item.GetComponent<PerkTag>().ClearPerk();
                //remove the name from the name dictionary
                equippedStats.displayedPerkNames.Remove(perksNOnThis[1]);
                Perks.perkNamePick.Remove(perksNOnThis[1]);
                break;
            }
        }
    }
    void GiveArmorToArmorSet(int armorIndex)
    {
        //give the visible armor slot the armor piece script
        armorManager.armorSetDisplay[armorIndex].AddComponent<EquippedArmor>();
        armorManager.armorSetDisplay[armorIndex].AddComponent<ArmorPiece>();
        //create new a list and array
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksNOnThis = new string[2];
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksVOnThis = new List<int>();
        //copy the names
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksNOnThis[0] = perksNOnThis[0];
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksNOnThis[1] = perksNOnThis[1];
        //copy the values
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksVOnThis.Add(perksVOnThis[0]);
        armorManager.armorSetDisplay[armorIndex].GetComponent<ArmorPiece>().perksVOnThis.Add(perksVOnThis[1]);
        //change the button text
        armorManager.armorSetDisplay[armorIndex].GetComponentInChildren<TextMeshProUGUI>().text = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        //hold this armorpiece in the temp array
        armorManager.tempArmorSet[armorIndex] = gameObject;
        armorManager.armorSetDisplay[armorIndex].GetComponent<EquippedArmor>().indexInSet = armorIndex;
    }
}
