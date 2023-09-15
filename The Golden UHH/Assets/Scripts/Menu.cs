using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;
    public GameObject TitleScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public GameObject[] playerlists;
    public TextMeshProUGUI playerListText;
    public Button startGameButton;
    public Animator[] anim;


    void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnConnectedToMaster ()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen (GameObject screen) 
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        TitleScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void OnCreateRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public void OnJoinRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }


    [PunRPC]
    public void UpdateLobbyUI ()
    {
        playerListText.text = "";
        int i = 0;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerlists[i].SetActive(true);
            playerlists[i].gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = player.NickName;
            i++;
        }
        for (i = 3; i >= PhotonNetwork.PlayerList.Length; i--)
        {
            if (playerlists[i].activeInHierarchy)
            {
                anim[i].SetTrigger("Exit");
            }
        }
        Invoke("DisablePlayerContainers", 1f);
        if(PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public override void OnJoinedRoom ()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public void OnLeaveLobbyButton ()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    
    public void OnMainScreenButton()
    {
        SetScreen(mainScreen);
    }

    public void OnStartGameButton ()
    {
        photonView.RPC("AnimateOut", RpcTarget.All);
        Invoke("StartTheGame", 1f);
    }
    [PunRPC]
    public void AnimateOut ()
    {
        for (int i = 0; i < anim.Length; i++)
        {
            if (i < playerlists.Length)
            {
                if (playerlists[i].activeInHierarchy)
                    anim[i].SetTrigger("Exit");
            }
            else
                anim[i].SetTrigger("Exit");
        }
    }

    public void StartTheGame()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }
    public void DisablePlayerContainers()
    {
        for (int i = 3; i >= PhotonNetwork.PlayerList.Length; i--)
        {
            if (playerlists[i].activeInHierarchy)
            {
                playerlists[i].SetActive(false);
            }
        }
    }
}

