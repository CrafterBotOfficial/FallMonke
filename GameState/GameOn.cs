namespace FallMonke.GameState;

public class GameOn : IGameState
{
    public GameStateEnum CheckGameState(int remainingPlayers, int remainingTiles)
    {
        if (remainingPlayers <= 1) return GameStateEnum.Finished;
        return GameStateEnum.GameOn;
    }
}
