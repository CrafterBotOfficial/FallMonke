namespace FallMonke.GamePlayer;

public class Participant(NetPlayer player)
{
    public NetPlayer Player { get; private set; } = player;
    public bool IsAlive { get; set; } = true;
    public ParticipantManager Manager { get; set; }
}
