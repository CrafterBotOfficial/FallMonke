namespace FallMonke;

public class Participant(NetPlayer player, VRRig rig, bool isAlive = true)
{
    public NetPlayer Player { get; private set; } = player;
    public bool IsAlive { get; set; } = isAlive;
    public ParticipantManager Manager { get; set; } = rig.AddComponent<ParticipantManager>();
}
