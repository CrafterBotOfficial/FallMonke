using System.Text;

namespace FallMonke.GameState;

public interface IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details);
    public void OnSwitchTo();

    public GameBoardText GetBoardText();
}

public struct GameBoardText(string title, StringBuilder body)
{
    public string Title = title;
    public StringBuilder Body = body;
}
