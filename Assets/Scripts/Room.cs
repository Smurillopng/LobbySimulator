/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using Photon.Realtime;

// Script que controla como os quartos são criados e suas configurações
// praticamente o mesmo script que ta no blackboard
public static class Room
{
    public static string CreateRoomName(string prefixName, int roomNumber)
    {
        return string.Concat(prefixName, roomNumber);
    }

    public static RoomOptions GetRoomOptions(int maxPlayers)
    {
        return new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = (byte) maxPlayers };
    }
}