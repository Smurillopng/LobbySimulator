using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text player1, pingP1, player2, pingP2;
    [SerializeField] private Config gameConfig;

    private Player _p1, _p2;
    private int _currentRoom;
    
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

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
                PhotonNetwork.Instantiate("cup_martini", new Vector3(-6, -2, 0), Quaternion.identity);
                break;
            case 2:
                this.Log("Player 2 joined");
                _p2 = PhotonNetwork.LocalPlayer;
                PhotonNetwork.Instantiate("cup_martini", new Vector3(6, -2, 0), Quaternion.identity);
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
            _p1 = null;
        else if (Equals(otherPlayer, _p2))
            _p2 = null;
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