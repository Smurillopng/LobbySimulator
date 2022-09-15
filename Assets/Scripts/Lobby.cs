/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

// Esse script é responsável por gerenciar a cena do lobby
public class Lobby : MonoBehaviourPunCallbacks
{
    #region Variáveis do Inspector

    [Header("Room")]
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private GameObject controlsPanel;
    
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

    #endregion

    #region Variáveis Privadas

    private Player _p1, _p2; // Armazena os dois jogadores do photon
    private GameObject _m1, _m2; // Armazena os copinhos que representam os jogadores
    private int _currentRoom; // Número da sala atual

    #endregion

    #region Métodos Unity

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        SetNames(); // Atualiza os nomes dos jogadores
        SetCups(); // Atualiza a posição dos copinhos

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

        if (!PhotonNetwork.IsConnected) return;
        this.Log("Trying to join room...");
        JoinRoom();
    }

    #endregion

    #region Métodos

    // Função que habilita e desabilita o painel de controles
    public void ShowControls() => controlsPanel.SetActive(!controlsPanel.activeSelf);

    // Função que atualiza os nomes
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
    
    // Função que atualiza a posição dos copinhos
    private void SetCups()
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.IsMasterClient && _m1 != null)
                _m1.transform.position = player1Spawn;
            else
            {
                if (_m2 == null) return;
                _m2.transform.position = player2Spawn;
            }
        }
    }
    
    // Função que cria ou entra nas salas
    private void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(
            Room.CreateRoomName(gameConfig.roomPrefixName, _currentRoom),
            Room.GetRoomOptions(gameConfig.maxPlayers),
            TypedLobby.Default);
    }
    private void GetNextRoom() => _currentRoom++; // Função que pega o número da próxima sala

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        this.Log("Failed to join room, trying to create...");
        GetNextRoom(); // Pega o número da próxima sala
        JoinRoom(); // Tenta criar a sala
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        this.Log("Failed to create room, trying to join...");
        GetNextRoom(); // Pega o número da próxima sala
        JoinRoom(); // Tenta entrar na sala
    }

    public override void OnJoinedRoom()
    {
        this.Log("Joined room");
        if (PhotonNetwork.InRoom)
            _currentRoom = gameConfig.roomStartNumber;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // Atualiza o nome da sala
        switch (PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                this.Log("Player 1 joined");
                _p1 = PhotonNetwork.LocalPlayer; // Pega o jogador local e armazena na variável
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
        switch (PhotonNetwork.CurrentRoom.PlayerCount) // Verifica quantos jogadores estão na sala
        {
            case 1: // Se tiver apenas um jogador na sala ele é o player 1
                this.Log("Player 1 joined");
                _p1 = newPlayer;
                break;
            case 2: // Se tiver dois jogadores na sala o player 2 é o jogador que entrou
                this.Log("Player 2 joined");
                _p2 = newPlayer;
                break;
        }
    }
    
    // Função que reseta os valores das variáveis caso o jogador saia da sala
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

    // Função que volta para o menu principal
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

    #endregion
}