namespace FallMonke;

public class Participant(NetPlayer player, VRRig rig, bool IsAlive = true)
{
    public bool IsAlive { get; set; } = IsAlive;
    public ParticipantManager Manager { get; set; } = rig.AddComponent<ParticipantManager>();
}
