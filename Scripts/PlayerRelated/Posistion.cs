using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posistion : MonoBehaviour
{
    //Takes damage inside of the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            FindObjectOfType<Player>().canTakeDamage = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
            FindObjectOfType<Player>().canTakeDamage = false;
    }
}
