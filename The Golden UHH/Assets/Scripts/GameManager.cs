using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickupTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;
    private int players_num = 1;

    public static GameManager instance;

    void Awake ()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
        players_num = playersInGame - 1;
        Debug.Log(players_num);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[players_num].position, Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer (int playerID)
    {
        return players.First(x => x.id == playerID);
    }

    public PlayerController GetPlayer (GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    [PunRPC] 
    public void GiveHat (int playerID, bool initialGive)
    {
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        playerWithHat = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;
    }

    public bool CanGetHat ()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else return false;
    }

    [PunRPC]
    void WinGame (int playerID)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerID);

        Invoke("GoBacktoMenu", 3.0f);

        GameUI.instance.SetWinText(player.photonPlayer.NickName);
    }

    void GoBacktoMenu ()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
