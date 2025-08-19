using UnityEngine;
using FallMonke.Hexagon;

namespace FallMonke;

public class ParticipantManager : MonoBehaviour
{
    public const float ELIMINATION_HEIGHT = 100;

    public Participant Info;
    public NetPlayer Player;
    public VRRig Rig;

    private void Start()
    {
    }

    // the idea behind this is everything will happen twice, once on locally only for the player and another for the master client
    private void Update()
    {
        // if (!HasAuthority(out bool isMine)) return;

        if (!Info.IsAlive)
            return;

        if (transform.position.y < ELIMINATION_HEIGHT)
        {
            // I died
            // CustomGameManager.Instance.Players.Remove(Info);
            Info.IsAlive = false;
            return;
        }


        if (TryRaycastToTerrain(out FallableHexagon hit) && !hit.IsFalling)
        {
            Main.Log("Yeeting platform");
            if (Player.IsLocal) hit.Fall();
            else if (NetworkSystem.Instance.IsMasterClient)
            {
                Main.Log($"player {Player.NickName} just fell a platform", BepInEx.Logging.LogLevel.Debug);
                hit.Fall();
                CustomGameManager.Instance.BroadcastController.FallPlatform(hit);
            }
        }
    }

    // private bool HasAuthority(out bool isLocal)
    // {
    //     isLocal = Rig.isLocal;
    //     return isLocal || NetworkSystem.Instance.IsMasterClient;
    // }

    private bool TryRaycastToTerrain(out FallableHexagon hitPlatform)
    {
        var direction = -Rig.transform.up;
        const float distance = .5f;
        const float width = .02f;

        Ray ray = new Ray(Rig.transform.position, direction);
        if (Physics.SphereCast(ray, width, out RaycastHit hit, distance)) // todo: add layer for hexagons
        {
            hitPlatform = hit.collider.gameObject.GetComponent<FallableHexagon>();
            return true;
        }

        hitPlatform = default;
        return false;
    }

    private void OnDisable()
    {
        // handle player leave
        Destroy(this);
    }
}
