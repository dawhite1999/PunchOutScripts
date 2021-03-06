using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject descriptionText;
    GameObject lockedText;
    [SerializeField]
    string descriptionEnglish;
    [SerializeField]
    string descriptionJapanese;

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.GetComponent<Text>().text = descriptionEnglish;
        if(gameObject.GetComponent<Ability>().isEquipped == true)
            lockedText.GetComponent<Text>().text = "Equipped";
        else if(gameObject.GetComponent<Button>().interactable == false)
            lockedText.GetComponent<Text>().text = "Locked";
        else
            lockedText.GetComponent<Text>().text = "";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.GetComponent<Text>().text = "";
        lockedText.GetComponent<Text>().text = "";
    }
    //called by button when equipping abilities
    public void EquipAbility()
    {
        int abilityCounter = 0;
        foreach (Ability ability in AbilityList.allAbilities)
        {
            if (ability.isEquipped == true)
            {
                if(ability.abilityName == gameObject.GetComponent<Ability>().abilityName)
                {
                    ability.isEquipped = false;
                    lockedText.GetComponent<Text>().text = "Unequipped";
                    break;
                }
                ability.isEquipped = false;
                gameObject.GetComponent<Ability>().isEquipped = true;
                lockedText.GetComponent<Text>().text = "Equipped";
                break;
            }
            else
                abilityCounter++;
            if(abilityCounter == AbilityList.allAbilities.Count)
            {
                gameObject.GetComponent<Ability>().isEquipped = true;
                lockedText.GetComponent<Text>().text = "Equipped";
            }          
        }     
    }
    //called in armormanager at start
    public void InitializeAbility()
    {
        descriptionText = GameObject.Find("AbilityDescription");
        lockedText = GameObject.Find("LockedText");
        AbilityList.allAbilities.Add(GetComponent<Ability>());
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = GetComponent<Ability>().abilityName;
    }
}
