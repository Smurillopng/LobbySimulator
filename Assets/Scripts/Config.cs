/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using UnityEngine;

// Esse script é responsável pelas configurações do quarto
// é praticamente o mesmo script que ta no blackboard 
[CreateAssetMenu(fileName = "Config", menuName = "Config")]
public class Config : ScriptableObject
{
    [Header("Room Config")]
    [HideInInspector ]public int maxPlayers = 2;
    [Tooltip("Prefixo para o nome da sala")]
    public string roomPrefixName = "Room_";
    [Tooltip("Numeração para o nome das salas")]
    public int roomStartNumber = 1;
}