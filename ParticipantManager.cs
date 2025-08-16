using UnityEngine; 

namespace FallMonke; 

public class ParticipantManager : MonoBehaviour 
{
    public const float ELIMINATION_HEIGHT = 100;
    
    public NetPlayer Player;
    public VRRig Rig;

    private void Start() {
    }
    
    private void Update() {
        if (transform.position.y < ELIMINATION_HEIGHT) {
            // I died
            return;
        }
    }

    private void OnDisable() {
        // handle player leave
    }
}
