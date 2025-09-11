namespace FallMonke.Networking;

// TODO: add fusion implimentation
public interface INetworkController 
{
    public void SetupEventHandler();

    public void FallPlatform(Hexagon.FallableHexagon tile);
    public void SendRandomSeed(int random);
    public void ShowNotification(string message);
    public void RequestStartGame();
    public void SendReportElimination();

    public Participant[] CreateParticipants();
    public int PlayersWithModCount();
    public void MakeModIdentifable();

    public void Cleanup();
}
