using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippableArmor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GetComponent<Button>().interactable == true)
            GetComponent<ArmorPiece>().EquipArmor(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(GameObject.Find("PlayerEquip") == true)
        {
            gameObject.GetComponent<CraftableArmor>().InitializeCA(true);
            gameObject.GetComponent<CraftableArmor>().DisplayStats(GetComponent<ArmorPiece>().perksNOnThis[0], GetComponent<ArmorPiece>().perksNOnThis[1], GetComponent<ArmorPiece>().perksVOnThis[0], GetComponent<ArmorPiece>().perksVOnThis[1], GetComponent<ArmorPiece>().BonusStatName, GetComponent<ArmorPiece>().BonusStatValue, GetComponent<ArmorPiece>().isBonus);
        }     
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameObject.Find("PlayerEquip") == true)
            gameObject.GetComponent<CraftableArmor>().ClearStats();
    }
}
