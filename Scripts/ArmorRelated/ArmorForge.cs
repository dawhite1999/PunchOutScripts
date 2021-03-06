using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorForge : MonoBehaviour
{
    //variables
    [SerializeField]
    int heatAdditionIntervals;
    [SerializeField]
    int heatSubtractionIntervals;
    [SerializeField]
    float weakFlameThreshold;
    [SerializeField]
    float strongFlameThreshold;
    int heatGauge;

    //gameobjects
    public GameObject heatMeter;
    public Slider heatSlider;
    public List<GameObject> beatsByDre;
    List<GameObject> storedBeats;
    GameObject beatHolder;

    //references
    ArmorManager armorManager;

    private void Start()
    {
        armorManager = FindObjectOfType<ArmorManager>();
    }
    private void Update()
    {
        HeatTempuratureChange();
        HeatCalculation();
    }
    public void PointCalculation(bool addPoints)
    {
        if (addPoints == true)
           armorManager.forgePoints = armorManager.forgePoints + armorManager.goodHitPoints + armorManager.heatPoints;
        else
        {
            armorManager.forgePoints -= armorManager.missPoints;
            if (armorManager.forgePoints < 0)
                armorManager.forgePoints = 0;
        }
    }
    //changes the color and points for the heat
    public void HeatCalculation()
    {
        if (heatSlider.value < weakFlameThreshold)
        {
            heatMeter.GetComponentInChildren<Image>().color = new Color(1, .2f ,0);
            armorManager.heatPoints = 1;
        }
        else if(heatSlider.value > strongFlameThreshold)
        {
            heatMeter.GetComponentInChildren<Image>().color = new Color(0, .9f, 1);
            armorManager.heatPoints = 1;
        }
        else
        {
            heatMeter.GetComponentInChildren<Image>().color = new Color(1, .6f, 0);
            armorManager.heatPoints = 3;
        }      
    }
    //what changes the value of the heat meter
    void HeatTempuratureChange()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            InvokeRepeating("AddHeat", 0, .2f);
            CancelInvoke("SubtractHeat");
        }         
        if (Input.GetKeyUp(KeyCode.Return))
        {
            InvokeRepeating("SubtractHeat", 0, .2f);
            CancelInvoke("AddHeat");
        }        
    }
    //functions to invoke
    void AddHeat()
    {
        heatSlider.value += heatAdditionIntervals;
    }
    void SubtractHeat()
    {
        heatSlider.value -= heatSubtractionIntervals;
    }

    public void InitializeForge(bool findThings)
    {
        if(findThings == true)
        {
            heatMeter = GameObject.Find("HeatSlider");
            heatSlider = GameObject.Find("HeatSlider").GetComponent<Slider>();
            storedBeats = beatsByDre;
        }
        else
            beatsByDre = storedBeats;
    }
    public IEnumerator ForgeComplete()
    {
        //find thing
        ArmorManager armorManager = FindObjectOfType<ArmorManager>();
        yield return new WaitForSeconds(2);
        armorManager.StartCoroutine(armorManager.EndForge());
        gameObject.SetActive(false);
    }
}
