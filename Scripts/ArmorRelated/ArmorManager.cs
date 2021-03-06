using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmorManager : MonoBehaviour
{
    //gameobjects
    GameObject normalForge;
    GameObject forgeList;
    GameObject CraftandEquipButtons;
    GameObject armorStatsBackground;
    GameObject equippedStatsBackground;
    GameObject playerEquip;
    Button goBackButton;
    TextMeshProUGUI gPointsOwned;
    TextMeshProUGUI mePointsOwned;
    TextMeshProUGUI iPointsOwned;
    public TextMeshProUGUI[] allStatTexts;
    public List<TextMeshProUGUI> equippedPerks;
    public GameObject[] armorSetDisplay = new GameObject[5];
    public GameObject[] tempArmorSet = new GameObject[5];
    [HideInInspector]
    public TextMeshProUGUI forgeStatus;
    [HideInInspector]
    public GameObject craftConfirm;
    [HideInInspector]
    public string forgeDifficulty;
    [HideInInspector]
    public GameObject tempArmorHolder;
    [SerializeField]
    GameObject[] abilities;
    GameObject abilityEquip;
    GameObject crafting;
    //variables
    public int forgePoints;
    public int goodHitPoints;
    public int missPoints;
    public int heatPoints;
    [SerializeField]
    List<GameObject> AllArmor;

    //References
    ArmorForge armorForge;
    InventoryArmor inventoryArmor;
    private void Start()
    {
        //find gameobjects/references
        normalForge = GameObject.Find("NormalForge");
        forgeList = GameObject.Find("ForgeList");
        forgeStatus = GameObject.Find("ForgeStatusText").GetComponent<TextMeshProUGUI>();
        CraftandEquipButtons = GameObject.Find("CEButtonsHolder");
        goBackButton = GameObject.Find("GoBackButton").GetComponent<Button>();
        armorStatsBackground = GameObject.Find("ArmorStatsBackground");
        craftConfirm = GameObject.Find("CraftConfirm");
        gPointsOwned = GameObject.Find("GamerPoints").GetComponent<TextMeshProUGUI>();
        mePointsOwned = GameObject.Find("MemePoints").GetComponent<TextMeshProUGUI>();
        iPointsOwned = GameObject.Find("IntelligencePoints").GetComponent<TextMeshProUGUI>();
        playerEquip = GameObject.Find("PlayerEquip");
        equippedStatsBackground = GameObject.Find("EquippedStatsBackground");
        armorForge = FindObjectOfType<ArmorForge>();
        inventoryArmor = FindObjectOfType<InventoryArmor>();
        abilityEquip = GameObject.Find("AbilityEquip");
        crafting = GameObject.Find("Crafting");
        foreach (GameObject ability in abilities)
        {
            TurnOnAbilities(ability);
        }
        abilityEquip.SetActive(false);
        foreach (CraftableArmor armor in FindObjectsOfType<CraftableArmor>())
        {
            armor.InitializeCA(false);
        }
        //setting values
        AddAllArmor();
        armorForge.InitializeForge(true);
        SendToEquipInv();
        inventoryArmor.BuildInv(true);
        FindObjectOfType<EquippedStats>().AddPerkGOs();
        Perks.InitializePerks();
        //turning stuff off
        normalForge.SetActive(false);
        forgeStatus.gameObject.SetActive(false);
        goBackButton.gameObject.SetActive(false);
        craftConfirm.SetActive(false);
        playerEquip.SetActive(false);
        crafting.SetActive(false);
        foreach (TextMeshProUGUI text in allStatTexts)
        {
            text.text = "";
        }
        armorStatsBackground.SetActive(false);
    }
    //called by button
    public void StartForge(string forgeType)
    {
        //set the difficulty
        forgeType = forgeDifficulty;
        tempArmorHolder.GetComponent<CraftableArmor>().isCrafted = true;
        StaticStats.PGamerPoints -= tempArmorHolder.GetComponent<CraftableArmor>().pointCosts[0];
        StaticStats.PMemePoints -= tempArmorHolder.GetComponent<CraftableArmor>().pointCosts[1];
        StaticStats.PIntelPoints -= tempArmorHolder.GetComponent<CraftableArmor>().pointCosts[2];
        SetOwnedPointsTexts();
        craftConfirm.SetActive(false);
        armorStatsBackground.SetActive(false);
        StartCoroutine(StartForgeCoroutine(forgeType));
    }
    public IEnumerator StartForgeCoroutine(string forgeVarient)
    {
        //activate text and deactivate buttons
        forgeStatus.gameObject.SetActive(true);
        forgeList.SetActive(false);
        goBackButton.gameObject.SetActive(false);
        forgeStatus.text = "Ready?";
        yield return new WaitForSeconds(1);
        forgeStatus.text = "GO!";
        yield return new WaitForSeconds(1);
        forgeStatus.gameObject.SetActive(false);
        //pick which forge difficulty to start
        switch (forgeVarient.ToLower())
        {
            case "easy":
                armorForge.InitializeForge(false);
                //normalForge.SetActive(true);
                break;
            case "normal":
                armorForge.InitializeForge(false);
                normalForge.SetActive(true);
                break;
            case "hard":
                armorForge.InitializeForge(false);
                //normalForge.SetActive(true);
                break;
        }
    }
    //called by button
    public void TurnOnCraftList()
    {
        crafting.SetActive(true);
        CraftandEquipButtons.SetActive(false);
        goBackButton.gameObject.SetActive(true);
        armorStatsBackground.SetActive(true);
        equippedStatsBackground.SetActive(false);
        foreach (GameObject armor in AllArmor)
        {
            armor.GetComponent<CraftableArmor>().CheckCosts();
        }
    }
    //called by button
    public void TurnOnEquip()
    {
        playerEquip.SetActive(true);
        CraftandEquipButtons.SetActive(false);
        goBackButton.gameObject.SetActive(true);
        armorStatsBackground.SetActive(true);
        equippedStatsBackground.SetActive(true);
    }
    public void AbilityEquipOn()
    {
        abilityEquip.SetActive(true);
        CraftandEquipButtons.SetActive(false);
        goBackButton.gameObject.SetActive(true);
        armorStatsBackground.SetActive(false);
        equippedStatsBackground.SetActive(false);
    }
    //called by button
    public void GoBack(string command)
    {
        switch(command)
        {
            case "BackFromList":
                if (crafting.activeSelf == true)
                    crafting.SetActive(false);
                if (playerEquip.activeSelf == true)
                    playerEquip.SetActive(false);
                if(abilityEquip.activeSelf == true)
                    abilityEquip.SetActive(false); 
                CraftandEquipButtons.SetActive(true);
                goBackButton.gameObject.SetActive(false);
                armorStatsBackground.SetActive(false);
                break;
            case "CloseConfirmation":
                craftConfirm.SetActive(false);
                break;
        }
    }
    public void SetOwnedPointsTexts()
    {
        gPointsOwned.text = "Gamer points owned: " + StaticStats.PGamerPoints;
        mePointsOwned.text = "Meme points owned: " + StaticStats.PMemePoints;
        iPointsOwned.text = "Intelligence points owned: " + StaticStats.PIntelPoints;
    }
    public IEnumerator EndForge()
    {
        //calculate the grade of armor
        tempArmorHolder.GetComponent<CraftableArmor>().ArmorRating();
        //add armor to inventory
        SendToEquipInv();
        //turn on text and turn it off
        forgeStatus.gameObject.SetActive(true);
        //turn off forge
        forgeStatus.text = "Forge Complete!";
        yield return new WaitForSeconds(3);
        forgeStatus.gameObject.SetActive(false);
        TurnOnCraftList();
    }
    //sends any crafted armor to the equip menu
    public void SendToEquipInv()
    {
        foreach (GameObject armor in AllArmor)
        {
            if (armor.GetComponent<CraftableArmor>().isCrafted == true && armor.GetComponent<CraftableArmor>().goodToAdd == true)
            {
                armor.AddComponent<EquippableArmor>();
                armor.GetComponent<CraftableArmor>().goodToAdd = false;
                inventoryArmor.craftedArmor.Add(armor);
                Debug.Log("Added " + armor.name);
            }          
        }
    }
    //adds all armor in the game to a list
    void AddAllArmor()
    {
        GameObject craftInv = GameObject.Find("CraftInventory");
        for (int i = 0; i < craftInv.transform.childCount; i++)
        {
            AllArmor.Add(craftInv.transform.GetChild(i).gameObject);  
        }
    }
    void TurnOnAbilities(GameObject ability)
    {
        switch (ability.name)
        {
            case "Pikay's Cowardice":
                if (PlayerWins.pikayVictory == false)
                    abilities[0].GetComponent<Button>().interactable = false;
                else
                    abilities[0].GetComponent<Button>().interactable = true;
                break;
            case "Vsauce's Hypothesis":
                if (PlayerWins.vSauceVictory == false)
                    abilities[1].GetComponent<Button>().interactable = false;
                else
                    abilities[1].GetComponent<Button>().interactable = true;
                break;
            case "Pewdiepie's Fist":
                if (PlayerWins.pewDiePieVictory == false)
                    abilities[2].GetComponent<Button>().interactable = false;
                else
                    abilities[2].GetComponent<Button>().interactable = true;
                break;
        }
        ability.GetComponent<AbilityButton>().InitializeAbility();
    }
}
