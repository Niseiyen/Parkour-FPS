using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private GameObject roomCam;

    [Header("UI")]
    [SerializeField] private GameObject NameUI;
    [SerializeField] private GameObject ConnectingUI;

    private string nickname = "Unnamed";

    [HideInInspector] public int kills;
    [HideInInspector] public int deaths;

    public string roomNameToJoin = "test";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    public void JoinRoomButtonPressed()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Connecting to server...");

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 8, 
                IsVisible = true, 
                IsOpen = true     
            };

            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, roomOptions, TypedLobby.Default);
            NameUI.SetActive(false);
            ConnectingUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Photon Network is not ready. Wait for OnConnectedToMaster or OnJoinedLobby.");
        }
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("Joined room and connected");

        roomCam.SetActive(false);

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            Debug.Log("Error setting hashes");
        }
    }
}
