using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PerkTag : MonoBehaviour
{
    public string perkNameTag;
    public int perkValue;
    public GameObject child;
    private void Start()
    {
        ShowPerk();
    }
    public void ShowPerk()
    {
        if (perkNameTag == "")
            return;
        else
        {
            GetComponent<TextMeshProUGUI>().text = perkNameTag;
            child.GetComponent<TextMeshProUGUI>().text = perkValue.ToString();
        }
    }
    public void ClearPerk()
    {
        perkNameTag = "";
        perkValue = 0;
        GetComponent<TextMeshProUGUI>().text = "";
        child.GetComponent<TextMeshProUGUI>().text = "";
    }
}
