using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class Lobby : MonoBehaviourPunCallbacks
{
    [Header("Players Name"), Space(2)]
    [SerializeField] private TMP_Text player1;
    [SerializeField] private TMP_Text player2;

    [Header("Players Ping"), Space(2)] 
    [SerializeField] private TMP_Text pingP1;
    [SerializeField] private TMP_Text pingP2;
    
    [Header("Players Spawn Position"), Space(2)]
    [SerializeField] private Vector3 player1Spawn;
    [SerializeField] private Vector3 player2Spawn;
    
    [Header("Room Settings"), Space(2)]
    [SerializeField] private Config gameConfig;
    
    private Player _p1, _p2;
    private GameObject _m1, _m2;
    private int _currentRoom;
    
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        SetNames();
        SetCups();

        if (_p1 is null)
        {
            player1.text = "Waiting for player...";
            pingP1.text = "...";
        }
        if (_p2 is null)
        {
            player2.text = "Waiting for player...";
            pingP2.text = "...";
        }

        if (player1.text != player2.text) return;
        player2.text = "Waiting for player...";
        pingP2.text = "...";
    }
    
    private void Start()
    {
        _currentRoom = gameConfig.roomStartNumber;
        
        if (PhotonNetwork.IsConnected)
        {
            this.Log("Trying to join room...");
            JoinRoom();
        }
    }

    private void SetNames()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.IsMasterClient)
            {
                _p1 = player;
                player1.text = _p1.NickName;
                pingP1.text = PhotonNetwork.GetPing().ToString();
            }
            else
            {
                _p2 = player;
                player2.text = _p2.NickName;
                pingP2.text = PhotonNetwork.GetPing().ToString();
            }
        }
    }

    private void SetCups()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.IsMasterClient && _m1 != null)
            {
                _m1.transform.position = player1Spawn;
            }
            else
            {
                if (_m2 == null) return;
                _m2.transform.position = player2Spawn;
            }
        }
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(
            Room.CreateRoomName(gameConfig.roomPrefixName, _currentRoom),
            Room.GetRoomOptions(gameConfig.maxPlayers),
            TypedLobby.Default);
    }
    private void GetNextRoom() => _currentRoom++;

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.Log("Failed to join room, trying to create...");
        GetNextRoom();
        JoinRoom();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        this.Log("Failed to create room, trying to join...");
        GetNextRoom();
        JoinRoom();
    }

    public override void OnJoinedRoom()
    {
        this.Log("Joined room");
        if (PhotonNetwork.InRoom)
            _currentRoom = gameConfig.roomStartNumber;

        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                this.Log("Player 1 joined");
                _p1 = PhotonNetwork.LocalPlayer;
                _m1 = PhotonNetwork.Instantiate("cup_martini",
                    _p1.IsMasterClient ? player1Spawn : player2Spawn, Quaternion.identity);
                break;
            case 2:
                this.Log("Player 2 joined");
                _p2 = PhotonNetwork.LocalPlayer;
                _m2 = PhotonNetwork.Instantiate("cup_martini",
                    _p2.IsMasterClient ? player1Spawn : player2Spawn, Quaternion.identity);
                break;
        }
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        this.Log("Player entered room");
        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                this.Log("Player 1 joined");
                _p1 = newPlayer;
                break;
            case 2:
                this.Log("Player 2 joined");
                _p2 = newPlayer;
                break;
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (Equals(otherPlayer, _p1))
        {
            _p1 = null;
            _m1 = _m2;
            _m2 = null;
        }
        else if (Equals(otherPlayer, _p2))
        {
            _p2 = null;
        }
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.Log("Room list updated");
        foreach (var info in roomList)
            this.Log($"Room {info.Name}, total players: " +
                     $"{info.PlayerCount}, Max Total players: {info.MaxPlayers}");
    }

    public void ExitToMainMenu()
    {
        if (Equals(PhotonNetwork.LocalPlayer, _p1))
            _p1 = null;
        
        else if (Equals(PhotonNetwork.LocalPlayer, _p2))
            _p2 = null;
        
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
        this.Log("Disconnected from server");
        this.Log("Returning to main menu");
    }
}