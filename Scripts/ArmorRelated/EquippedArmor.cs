using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class EquippedArmor : MonoBehaviour, IPointerClickHandler
{
    public int indexInSet;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(GetComponent<Button>().interactable == true)
        {
            GetComponent<ArmorPiece>().EquipArmor(false);
            Destroy(gameObject.GetComponent<ArmorPiece>());
            Destroy(gameObject.GetComponent<EquippedArmor>());
            GetComponentInChildren<TextMeshProUGUI>().text = "";
            FindObjectOfType<ArmorManager>().tempArmorSet[indexInSet].GetComponent<Button>().interactable = true;
            FindObjectOfType<ArmorManager>().tempArmorSet[indexInSet] = null;
        }
    }
}
