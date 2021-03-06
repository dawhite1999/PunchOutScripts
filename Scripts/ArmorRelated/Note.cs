using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //find thing
        ArmorForge armorForge = GetComponentInParent<ArmorForge>();
        //if hit notekiller
        if (collision.GetComponent<CircleCollider2D>() != null)
        {
            //remove the first item in the list and turn it off
            armorForge.beatsByDre.RemoveAt(0);
            //if its the last one turn the forge game off
            if (armorForge.beatsByDre.Count == 0)
                armorForge.StartCoroutine(armorForge.ForgeComplete());
            gameObject.SetActive(false);
        }          
    }
}
