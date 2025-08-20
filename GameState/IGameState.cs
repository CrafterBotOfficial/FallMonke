namespace FallMonke.GameState;

public interface IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details);
    public void OnSwitchTo();
}
