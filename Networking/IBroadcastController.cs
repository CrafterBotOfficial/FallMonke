namespace FallMonke.Networking;

// todo: add fusion implimentation
// todo: rename
public interface IBroadcastController
{
    public void SetupEventHandler();

    public void FallPlatform(Hexagon.FallableHexagon tile);
    public void SendRandomSeed(int random);
    public void ShowNotification(string message);
    public void RequestStartGame();

    public Participant[] CreateParticipants();
    public int PlayersWithModCount();
    public void MakeModIdentifable();

    public void Cleanup();
}
