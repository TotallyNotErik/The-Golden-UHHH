using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public void OnTriggerEnter (Collider col) 
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if(col.gameObject.GetComponent<PlayerController>().hatObject.activeInHierarchy)
            {
                col.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                GameManager.instance.Pickup.SetActive(true);
                col.gameObject.SetActive(false);
            }
            GameManager.instance.playersLeft--;
        }
    }
}
