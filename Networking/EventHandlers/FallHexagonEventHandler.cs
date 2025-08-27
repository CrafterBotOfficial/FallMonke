using UnityEngine;

namespace FallMonke.Networking.EventHandlers;

public class FallHexagonEventHandler : IEventHandler
{
    public void OnEvent(NetPlayer sender, object data)
    {
        var manager = (CustomGameManager)CustomGameManager.instance;
        if (manager.CurrentState != GameState.GameStateEnum.GameOn)
            return;

        if (sender.IsLocal)
            return;

        if (data is not int tileIndex)
        {
            Main.Log("Bad event, not expected data type", BepInEx.Logging.LogLevel.Warning);
            return;
        }

        var tile = WorldManager.Instance.GetTileByIndex(tileIndex);

        VRRig rig = manager.FindPlayerVRRig(sender);
        const float maxDistance = 10;
        if (Vector3.Distance(rig.transform.position, tile.transform.position) > maxDistance)
        {
            Main.Log("Tile to fare away, likely a cheater.", BepInEx.Logging.LogLevel.Warning);
            return;
        }

        Main.Log("Falling tile, told to from other player", BepInEx.Logging.LogLevel.Debug);
        tile.Fall();
    }
}
