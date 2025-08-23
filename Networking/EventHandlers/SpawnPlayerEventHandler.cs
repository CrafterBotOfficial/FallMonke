namespace FallMonke.Networking.EventHandlers;

public class SpawnPlayerEventHandler : IEventHandler
{
    public void OnEvent(NetPlayer sender, object data)
    {
        if (sender.IsMasterClient && data is int random)
        {
            TeleportController.TeleportToGame(random);
            return;
        }
        Main.Log("Bad event", BepInEx.Logging.LogLevel.Warning);
    }
}
