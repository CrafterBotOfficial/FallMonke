namespace FallMonke.Networking.EventHandlers;

public class SpawnPlayerEventHandler : IEventHandler
{
    public void OnEvent(NetPlayer sender, object data)
    {
        if (sender.IsMasterClient && data is int seed)
        {
            TeleportController.TeleportToGame(seed);
            return;
        }
        Main.Log("Bad event", BepInEx.Logging.LogLevel.Warning);
    }
}
