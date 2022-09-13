using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Config")]
public class Config : ScriptableObject
{
    [Header("Room Config")]
    [Tooltip("Máximo de jogadores na sala")]
    public int maxPlayers = 2;
    [Tooltip("Prefixo para o nome da sala")]
    public string roomPrefixName = "Room_";
    [Tooltip("Numeração para o nome das salas")]
    public int roomStartNumber = 1;
}