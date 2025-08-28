namespace FallMonke;

public class Participant(NetPlayer player)
{
    public NetPlayer Player { get; private set; } = player;
    public bool IsDead { get; set; }
    public bool IsAlive { get { return !IsDead; } }
    public ParticipantManager Manager { get; set; }
}
