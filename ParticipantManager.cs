using UnityEngine;
using FallMonke.Hexagon;

namespace FallMonke;

public class ParticipantManager : MonoBehaviour
{
    public Participant Info;
    public VRRig Rig;

    private Transform[] bodyParts;

    private void Start()
    {
        bodyParts = [
            Rig.transform,
            Rig.leftHandTransform,
            Rig.rightHandTransform
        ];
    }

    // the idea behind this is everything will happen twice, once on locally only for the player and another for the master client
    private async void Update()
    {
        var manager = (CustomGameManager)GorillaGameManager.instance;// https://www.youtube.com/shorts/pFB5F-fS_Y4

        if (!Info.IsAlive)
            return;

        if (transform.position.y < WorldManager.EliminationHeight)
        {
            // I died
            Info.IsAlive = false;

            if (Info.Player.IsLocal)
            {
                await System.Threading.Tasks.Task.Delay(2500); // ensures other players recognize this one has been dead
                manager.NotificationHandler.ShowNotification("You have been eliminated");
                TeleportController.TeleportToLobby();
            }
            else manager.NotificationHandler.ShowNotification($"{Info.Player.SanitizedNickName} has been eliminated");
            return;
        }

        // change: Master no longer manages tiles, each player does then tells everyone else when fell
        if (Info.Player.IsLocal && !manager.CooldownInAffect)
            foreach (var bodyPart in bodyParts)
                if (TryRaycastToTerrain(bodyPart.position,
                                        bodyPart == bodyParts[0] ? .3f : .1f, // testing code, should refactor later
                                        out FallableHexagon hit) && !hit.IsFalling) // todo: we should have a fatter spherecast for the main body so a player can sit on the ledge
                {
                    Main.Log("Yeeting platform, detector: " + bodyPart.name, BepInEx.Logging.LogLevel.Debug);
                    hit.Fall();
                    manager.BroadcastController.FallPlatform(hit);
                }
    }

    // todo: we should also raycast from both hands down, otherwise players can cheat
    private bool TryRaycastToTerrain(Vector3 origin, float radius, out FallableHexagon hitPlatform)
    {
        Vector3 offset = origin + Vector3.up * 0.1f;
        const float distance = 0.5f;

        bool hitSomething = Physics.SphereCast(offset, radius, Vector3.down, out RaycastHit hit, distance, LayerMask.GetMask("Gorilla Object"));
        if (hitSomething && hit.transform.gameObject.TryGetComponent(out FallableHexagon tile))
        {
            hitPlatform = tile;
            return true;
        }

        hitPlatform = default;
        return false;
    }

    // handle player leave
    private void OnDisable()
    {
        Info.IsAlive = false;
        Destroy(this);
    }
}
