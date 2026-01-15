// todo:
// make scoreboard actually scale with the amount of players

using System.Linq;

namespace FallMonke.GameState;

public class GameOn : IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details)
    {
        var manager = (CustomGameManager)GorillaGameManager.instance;

        if (details.RemainingPlayers == GameConfig.MIN_PLAYERS_TO_CONTINUE)
        {
            string winText = $"{GetWinner().Player.SanitizedNickName} wins!";
            manager.NetworkController.ShowNotification(winText);
            return GameStateEnum.Finished;
        }

        if (details.RemainingPlayers < GameConfig.MIN_PLAYERS_TO_CONTINUE)
        {
            Main.Log("Not enough players to continue the game.");
            manager.NetworkController.ShowNotification("Game over!");
            return GameStateEnum.Finished;
        }

        return GameStateEnum.GameOn;
    }

    public void OnSwitchTo()
    {
        Main.Log("GameOn: Im getting called, things are starting up");
        var manager = (CustomGameManager)GorillaGameManager.instance;
        manager.CreateParticipants();
        Main.Log($"{manager.Players.Length}/{NetworkSystem.Instance.AllNetPlayers.Length} players Participanting");
        manager.NotificationHandler.ShowNotification("Game on!");

        if (NetworkSystem.Instance.IsMasterClient)
        {
            Main.Log("Looks like its up to me to decide where everyone has to go.");
            int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            manager.NetworkController.SendTeleportToGame(seed);
        }

        manager.StartCoroutine(nameof(CustomGameManager.StartGameCooldown));
    }

    public GameBoardText GetBoardText()
    {
        var manager = CustomGameManager.GetInstance();
        if (manager.Players.IsNullOrEmpty())
            return new GameBoardText("An error occured, please reset.", default);

        var stringBuilder = new System.Text.StringBuilder();
        var players = manager.Players.OrderBy(player => !player.IsDead);
        foreach (var player in players)
            if (player.IsDead)
                stringBuilder.AppendLine($"<align=\"left\"><color=#ff0800>{player.Player.SanitizedNickName}</color>");
            else
                stringBuilder.AppendLine($"<align=\"left\">{player.Player.SanitizedNickName}");

        return new GameBoardText("Remaining Players", stringBuilder);
    }

    private Participant GetWinner()
    {
        return CustomGameManager.GetInstance().Players.First(x => x.IsAlive);
    }
}
