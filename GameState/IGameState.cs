using System.Text;

namespace FallMonke.GameState;

public interface IGameState
{
    public GameStateEnum CheckGameState(GameStateDetails details); // only called on master
    public void OnSwitchTo(); // called on all clients

    public GameBoardText GetBoardText(); // called on all clients
}

public struct GameBoardText(string title, StringBuilder body)
{
    public string Title = title;
    public StringBuilder Body = body;
}
