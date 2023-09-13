using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;
    public Material goldEgg;
    public Material def;
    public Vector3[] spawnLocations;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine && GameManager.instance.started)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();
            if (hatObject.activeInHierarchy)
                curHatTime += Time.deltaTime;
        }
    }

    void Move ()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    void TryJump ()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void Initialize (Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        if (!photonView.IsMine)
            rig.isKinematic = true;

        Invoke("teleportToStart", 5 + 0.5f * id);
    }

    [PunRPC]
    public void teleportToStart()
    {
        this.transform.position = spawnLocations[id - 1];
        anim.SetTrigger("Spawned");
    }

    public void SetHat (bool hasHat)
    {
        hatObject.SetActive(hasHat);
        if (hasHat)
            this.transform.gameObject.GetComponent<Renderer>().material = goldEgg;
        else
            this.transform.gameObject.GetComponent<Renderer>().material = def;
    }
    void OnCollisionEnter (Collision collision)
    {
        if (!photonView.IsMine)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                if(GameManager.instance.CanGetHat())
                {
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
        if (collision.gameObject.CompareTag("Pickup"))
        {
            Debug.Log("Yo");
            GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, true);
            Destroy(collision.gameObject);
        }
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(curHatTime);
        else if (stream.IsReading)
            curHatTime = (float)stream.ReceiveNext();
    }
}
