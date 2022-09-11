using Photon.Pun;
using TMPro;
using UnityEngine;
using Utils;

public class Pun : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField playerName;

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby()
    {
        this.Log("Joined Lobby");
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
        PhotonNetwork.LoadLevel("Lobby");
    }
}