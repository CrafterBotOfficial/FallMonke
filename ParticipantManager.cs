using UnityEngine;
using FallMonke.Hexagon;

namespace FallMonke;

public class ParticipantManager : MonoBehaviour
{
    public Participant Info;
    public VRRig Rig;

    // the idea behind this is everything will happen twice, once on locally only for the player and another for the master client
    private async void Update()
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
                await System.Threading.Tasks.Task.Delay(2500);
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
        Vector3 origin = GorillaLocomotion.GTPlayer.Instance.bodyCollider.bounds.center + Vector3.up * 0.1f;
        float distance = 0.5f;

        bool hitSomething = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, distance, LayerMask.GetMask("Gorilla Object"));

        if (hitSomething && hit.transform.gameObject.TryGetComponent(out FallableHexagon tile))
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
