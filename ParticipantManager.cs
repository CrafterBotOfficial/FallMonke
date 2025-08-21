using UnityEngine;
using FallMonke.Hexagon;

namespace FallMonke;

public class ParticipantManager : MonoBehaviour
{
    public Participant Info;
    public VRRig Rig;

    // the idea behind this is everything will happen twice, once on locally only for the player and another for the master client
    private void Update()
    {
        // if (!HasAuthority(out bool isMine)) return;

        if (!Info.IsAlive)
            return;

        if (transform.position.y < WorldManager.EliminationHeight)
        {
            // I died
            Info.IsAlive = false;
            var manager = CustomGameManager.Instance; // https://www.youtube.com/shorts/pFB5F-fS_Y4
            if (Info.Player.IsLocal)
            {
                manager.NotificationHandler.ShowNotification("You have been eliminated");
                TeleportController.TeleportToLobby();
            }
            else manager.NotificationHandler.ShowNotification($"{Info.Player.SanitizedNickName} has been eliminated");
            return;
        }

        // change: Master no longer manages tiles, each player does then tells everyone else when fell
        if (Info.Player.IsLocal && TryRaycastToTerrain(out FallableHexagon hit) && !hit.IsFalling)
        {
            Main.Log("Yeeting platform");
            hit.Fall();
            CustomGameManager.Instance.BroadcastController.FallPlatform(hit);
            // else if (NetworkSystem.Instance.IsMasterClient)
            // {
            //     Main.Log($"player {Player.NickName} just fell a platform", BepInEx.Logging.LogLevel.Debug);
            // }
        }
    }

    // todo: we should also raycast from both hands down, otherwise players can cheat
    private bool TryRaycastToTerrain(out FallableHexagon hitPlatform)
    {
        const float distance = .5f;
        const float width = .02f;

        Ray ray = new Ray(Rig.transform.position, Vector3.down);
        if (Physics.SphereCast(ray, width, out RaycastHit hit, distance) && hit.collider.GetComponentInChildren<FallableHexagon>() is FallableHexagon tile)
        {
            hitPlatform = tile;
            return true;
        }

        hitPlatform = default;
        return false;
    }

    private void OnDisable()
    {
        // handle player leave
        Info.IsAlive = false;
        Destroy(this);
    }
}
