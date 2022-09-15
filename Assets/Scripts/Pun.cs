/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using Photon.Pun;
using TMPro;
using UnityEngine;
using Utils;

// Esse script é responsável por conectar o jogador ao servidor Photon e por gerenciar a cena MainMenu
public class Pun : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField playerName;

    // Conecta o jogador ao servidor Photon ao iniciar o jogo
    public void Start() => PhotonNetwork.ConnectUsingSettings();

    #region Métodos

    // Coloca o jogador em um lobby
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => this.Log("Joined Lobby"); // Debug

    // Carrega a cena Lobby e salva o nome do jogador
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
        PhotonNetwork.Disconnect(); // Desconecta o jogador do servidor Photon
        this.Log("Exiting game");
        Application.Quit(); // Fecha o jogo
    }

    #endregion
    
}