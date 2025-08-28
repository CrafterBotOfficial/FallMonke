using System.Linq;

namespace FallMonke.Networking.EventHandlers;

public class EliminatePlayerEventHandler : IEventHandler
{
    public void OnEvent(NetPlayer sender, object data)
    {
        var manager = (CustomGameManager)CustomGameManager.instance;

        if (manager.CurrentState != GameState.GameStateEnum.GameOn)
        {
            Main.Log("Report after game over, likely a tie game.", BepInEx.Logging.LogLevel.Warning);
            return;
        }

        var participant = manager.Players.FirstOrDefault(x => x.Player == sender);

        if (participant == null)
        {
            Main.Log("Bad event, sender isn't participant", BepInEx.Logging.LogLevel.Warning);
            return;
        }

        if (participant.IsDead)
        {
            Main.Log(sender.NickName + " already eliminated", BepInEx.Logging.LogLevel.Warning);
            return;
        }

        participant.Manager.Eliminate();

        if (NetworkSystem.Instance.IsMasterClient)
        {
            manager.InfrequentUpdate();
        }
    }
}
