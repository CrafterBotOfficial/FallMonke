using System.Linq;

namespace FallMonke.GameState;

public class GameOn : IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        var manager = (CustomGameManager)CustomGameManager.instance;
        if (details.RemainingPlayers == 1)
        {
            string winText = $"{GetWinner().Player.SanitizedNickName} wins!";
            manager.BroadcastController.ShowNotification(winText);
            return GameStateEnum.Finished;
        }

        if (details.RemainingPlayers < 1)
        {
            Main.Log("Not enough players to continue the game.");
            manager.BroadcastController.ShowNotification("Game over!");
            return GameStateEnum.Finished;
        }

        return GameStateEnum.GameOn;
    }

    public void OnSwitchTo()
    {
        Main.Log("GameOn: Im getting called, things are starting up");
        var manager = (CustomGameManager)CustomGameManager.instance;
        manager.CreateParticipants();
        Main.Log($"{manager.Players.Length}/{NetworkSystem.Instance.AllNetPlayers.Length} players Participanting");
        manager.NotificationHandler.ShowNotification("Game on!");

        if (NetworkSystem.Instance.IsMasterClient)
        {
            Main.Log("Looks like its up to me to decide where everyone has to go.");
            int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            manager.BroadcastController.SendRandomSeed(seed);
        }

        // test code - lets see if this feels good or not
        manager.CooldownInAffect = true;
        new System.Threading.Thread(async () =>
        {
            await System.Threading.Tasks.Task.Delay(3000);
            manager.CooldownInAffect = false;
        }).Start();
    }

    public GameBoardText GetBoardText()
    {
        var manager = (CustomGameManager)CustomGameManager.instance;
        if (manager.Players.IsNullOrEmpty())
            return new();

        var stringBuilder = new System.Text.StringBuilder();
        var players = manager.Players.OrderBy(player => player.IsAlive);
        foreach (var player in players)
            if (player.IsAlive)
                stringBuilder.AppendLine($"<align=\"left\">{player.Player.SanitizedNickName}");
            else
                stringBuilder.AppendLine($"<align=\"left\"><color=#ff0800>{player.Player.SanitizedNickName}</color>");

        return new GameBoardText("Remaining Players", stringBuilder);
    }

    private Participant GetWinner()
    {
        return ((CustomGameManager)CustomGameManager.instance).Players.First(x => x.IsAlive);
    }
}
