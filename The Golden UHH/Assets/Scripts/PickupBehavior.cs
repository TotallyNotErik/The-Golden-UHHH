using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PickupBehavior : MonoBehaviourPunCallbacks
{

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, GameManager.instance.GetPlayer(collision.gameObject).id, false);
            //Destroy(this.gameObject);
        }

    }
}
