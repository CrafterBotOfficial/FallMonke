namespace FallMonke;

public class Participant(NetPlayer player)
{
    public readonly NetPlayer Player = player;
    public bool IsDead;
    public bool IsAlive => !IsDead;
    public ParticipantManager Manager;
}
