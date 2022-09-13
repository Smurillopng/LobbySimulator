using Photon.Pun;
using TMPro;
using UnityEngine;
using Utils;

public class Pun : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField playerName;

    public void Start() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => this.Log("Joined Lobby");

    public void LoadLobby()
    {
        if(playerName != null && !string.IsNullOrEmpty(playerName.text))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName.text;
            PhotonNetwork.LoadLevel("Lobby");
        }
        else
            this.Log("Please enter a name");
    }
    
    public void ExitGame()
    {
        PhotonNetwork.Disconnect();
        this.Log("Exiting game");
        Application.Quit();
    }
}