using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteZone : MonoBehaviour
{
    //references
    ArmorForge armorForge;
    //gameobjects
    public Image zoneHighlight;
    //variables
    bool canHitNote = false;

    // Start is called before the first frame update
    void Start()
    {
        armorForge = FindObjectOfType<ArmorForge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            NoteHitCheck();
        if (Input.GetKeyUp(KeyCode.X))
            zoneHighlight.gameObject.SetActive(false);
    }
    void NoteHitCheck()
    {
        //if touching a note, add points and set highlight color
        if (canHitNote == true)
        {
            armorForge.PointCalculation(true);
            zoneHighlight.gameObject.SetActive(true);
            zoneHighlight.color = Color.green;
        }
        //if not touching a note, subtract points and set highlight color
        else
        {
            zoneHighlight.gameObject.SetActive(true);
            zoneHighlight.color = Color.red;
            armorForge.PointCalculation(false);
        }       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if you collide with a note, turn on bool
        if(collision.GetComponent<Note>() != null)
            canHitNote = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if a note leaves you, turn off bool
        if (collision.GetComponent<Note>() != null)
            canHitNote = false;
    }
}
