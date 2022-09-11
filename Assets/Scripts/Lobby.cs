using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text pingP1, player1, pingP2, player2;
    
    private Player _p1, _p2;
    
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.IsMasterClient)
            {
                _p1 = player;
                player1.text = player.NickName;
                pingP1.text = PhotonNetwork.GetPing().ToString();
            }
            else
            {
                _p2 = player;
                player2.text = player.NickName;
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
        if (player1.text == player2.text)
        {
            player2.text = "Waiting for player...";
            pingP2.text = "...";
        }
    }
    

    private void Start()
    {
        this.Log("Trying to join room...");
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.Log("Failed to join random room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        this.Log("Joined room");

        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                this.Log("Player 1 joined");
                _p1 = PhotonNetwork.LocalPlayer;
                break;
            case 2:
                this.Log("Player 2 joined");
                _p2 = PhotonNetwork.LocalPlayer;
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
        }
        else if (Equals(otherPlayer, _p2))
        {
            _p2 = null;
        }   
    }

    public void ExitToMainMenu()
    {
        if (Equals(PhotonNetwork.LocalPlayer, _p1))
        {
            _p1 = null;
        }
        else if (Equals(PhotonNetwork.LocalPlayer, _p2))
        {
            _p2 = null;
        }

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
        this.Log("Disconnected from server");
        this.Log("Returning to main menu");
    }
}