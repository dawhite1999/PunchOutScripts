using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class CraftableArmor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI perk1Text;
    TextMeshProUGUI perk2Text;
    TextMeshProUGUI perkTextHeader;
    TextMeshProUGUI gameCostText;
    TextMeshProUGUI memeCostText;
    TextMeshProUGUI intelCostText;
    TextMeshProUGUI errorText;
    ArmorManager armorManager;
    ArmorPiece armorPiece;
    [SerializeField]
    int maxForgePoints;
    [Header("Order from top to bottom: Gamer, Meme, Int")]
    public int[] pointCosts;
    [Header("Difficulties are Easy, Normal, and Hard")]
    [SerializeField]
    string difficulty;
    public bool isCrafted = false;
    [HideInInspector]
    public bool goodToAdd = true;
    public void InitializeCA(bool inEquipMenu)
    {
        if(inEquipMenu == false)
        {
            armorManager = FindObjectOfType<ArmorManager>();
            armorPiece = GetComponent<ArmorPiece>();
            perk1Text = GameObject.Find("PerkText").GetComponent<TextMeshProUGUI>();
            perk2Text = GameObject.Find("PerkText2").GetComponent<TextMeshProUGUI>();
            perkTextHeader = GameObject.Find("PerkTextHeader").GetComponent<TextMeshProUGUI>();
            gameCostText = GameObject.Find("GPointsCost").GetComponent<TextMeshProUGUI>();
            memeCostText = GameObject.Find("MPointsCost").GetComponent<TextMeshProUGUI>();
            intelCostText = GameObject.Find("IPointsCost").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            perk1Text = GameObject.Find("PerkText").GetComponent<TextMeshProUGUI>();
            perk2Text = GameObject.Find("PerkText2").GetComponent<TextMeshProUGUI>();
            perkTextHeader = GameObject.Find("PerkTextHeader").GetComponent<TextMeshProUGUI>();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isCrafted == false)
        {
            DisplayStats(armorPiece.perksNOnThis[0], armorPiece.perksNOnThis[1], armorPiece.perksVOnThis[0], armorPiece.perksVOnThis[1], armorPiece.BonusStatName, armorPiece.BonusStatValue, armorPiece.isBonus);
            DisplayCosts(pointCosts[0], pointCosts[1], pointCosts[2], pointCosts[3]);
            armorManager.SetOwnedPointsTexts();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(isCrafted == false)
        {
            foreach (TextMeshProUGUI text in armorManager.allStatTexts)
            {
                text.text = "";
            }
        }
    }
    public void DisplayStats(string perk1, string perk2, int value1, int value2, string bonusPerk, int bonusValue, bool? hasBonus)
    {
        perkTextHeader.text = "This armor's perks";
        //if bonus is positive
        if(hasBonus == true)
        {
            //add bonus value to the corresponding perk
            if(bonusPerk == perk1)
            {
                perk1Text.text = perk1 + ": " + value1 + " + (" + bonusValue + ")";
                if (perk2 == "")
                    perk2Text.text = perk2;
                else
                    perk2Text.text = perk2 + ": " + value2;
            }
            else if(bonusPerk == perk2)
            {
                perk1Text.text = perk1 + ": " + value1;
                perk2Text.text = perk2 + ": " + value1 + " + (" + bonusValue + ")";
            }       
        }
        //if bonus is negative
        else if (hasBonus == false)
        {
            //subtract bonus value from the corresponding perk
            if (bonusPerk == perk1)
            {
                perk1Text.text = perk1 + ": " + value1 + " - (" + bonusValue + ")";
                if (perk2 == "")
                    perk2Text.text = perk2;
                else
                    perk2Text.text = perk2 + ": " + value2;
            }
            else if (bonusPerk == perk2)
            {
                perk1Text.text = perk1 + ": " + value1;
                perk2Text.text = perk2 + ": " + value1 + " - (" + bonusValue + ")";
            }
        }
        //if bonus is null
        else
        {
            perk1Text.text = perk1 + ": " + value1;
            if (perk2 == "")
                perk2Text.text = perk2;
            else
                perk2Text.text = perk2 + ": " + value2;
        }

    }
    public void ClearStats()
    {
        perkTextHeader.text = "";
        perk1Text.text = "";
        perk2Text.text = "";
    }
    void DisplayCosts(int gamerCost, int memeCost, int intelCost, int musicCost)
    {
        gameCostText.text = "Gamer points required: " + gamerCost;
        memeCostText.text = "Meme points required: " + memeCost;
        intelCostText.text = "Intelligence points required: " + intelCost;
    }
    public void ForgeConfirmation()
    {
        if (StaticStats.PGamerPoints >= pointCosts[0] && StaticStats.PMemePoints >= pointCosts[1] && StaticStats.PIntelPoints >= pointCosts[2] && GetComponent<EquippableArmor>() == null)
        {
            armorManager.craftConfirm.SetActive(true);
            armorManager.forgeDifficulty = difficulty;
            armorManager.tempArmorHolder = gameObject;
        }
    }
    public void CheckCosts()
    {
        if (StaticStats.PGamerPoints >= pointCosts[0] && StaticStats.PMemePoints >= pointCosts[1] && StaticStats.PIntelPoints >= pointCosts[2] && isCrafted == false)
            gameObject.GetComponent<Button>().interactable = true;
        else
            gameObject.GetComponent<Button>().interactable = false;
    }
    public void ArmorRating()
    {
        if(armorManager.forgePoints <= .3f * maxForgePoints)
        {
            //make bad armor
            armorPiece.isBonus = false;
            isCrafted = true;
            armorManager.SendToEquipInv();
        }
        if(armorManager.forgePoints > .3f * maxForgePoints && armorManager.forgePoints <= .8f * maxForgePoints)
        {
            //make normal armor
            armorPiece.isBonus = null;
            isCrafted = true;
            armorManager.SendToEquipInv();
        }
        if(armorManager.forgePoints > .8f * maxForgePoints)
        {
            //make good armor
            armorPiece.isBonus = true;
            isCrafted = true;
            armorManager.SendToEquipInv();
        }
    }
}
