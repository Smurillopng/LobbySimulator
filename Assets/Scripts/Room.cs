using Photon.Realtime;

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