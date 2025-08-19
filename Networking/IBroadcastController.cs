namespace FallMonke.Networking;

// todo: add fusion implimentation
// todo: rename
public interface IBroadcastController
{
    public void FallPlatform(Hexagon.FallableHexagon tile);
    public Participant[] CreateParticipants();
    public void MakeModIdentifable(); // not sure how this can be done with fusion
}
