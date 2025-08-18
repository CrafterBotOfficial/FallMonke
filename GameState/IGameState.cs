namespace FallMonke.GameState;

public interface IGameState
{
    public GameStateEnum CheckGameState(int remainingPlayers, int remainingTiles);
}
