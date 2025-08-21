namespace FallMonke.Networking;

// todo: add fusion implimentation
// todo: rename
public interface IBroadcastController
{
    public void SetupEventHandler();
    public void FallPlatform(Hexagon.FallableHexagon tile);
    public void SendRandomSeed(int random);
    public void ShowNotification(string message);

    public Participant[] CreateParticipants();
    public void MakeModIdentifable(); // not sure how this can be done with fusion

    public void Cleanup();
}
