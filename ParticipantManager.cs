using UnityEngine;
using FallMonke.Hexagon;

namespace FallMonke.GamePlayer;

public class ParticipantManager : MonoBehaviour
{
    public Participant Info;
    public VRRig Rig;

    private LayerMask hexagonLayer = LayerMask.GetMask("Gorilla Object");
    private Transform[] rigHands;

    private void Start()
    {
        rigHands = [
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
        {
            FallableHexagon hexagon;

            // body down raycast
            if (TryRaycastToTerrain(Rig.transform.position, .5f, out hexagon))
            {
                OnTouchHexagon(manager, hexagon);
                return;
            }

            // hands - only if nothing under body
            if (TryRaycastToTerrain(Rig.transform.position, .05f, out hexagon))
            {
                OnTouchHexagon(manager, hexagon);
            }
        }
    }

    private void OnTouchHexagon(CustomGameManager manager, FallableHexagon hexagon)
    {
        Main.Log("Yeeting platform", BepInEx.Logging.LogLevel.Debug);
        hexagon.Fall();
        manager.BroadcastController.FallPlatform(hexagon);
    }

    private bool TryRaycastToTerrain(Vector3 origin, float distance, out FallableHexagon hitPlatform)
    {
        Vector3 offset = origin + Vector3.up * 0.1f;
        const float radius = 0.1f;

        bool hitSomething = Physics.SphereCast(offset, radius, Vector3.down, out RaycastHit hit, distance, hexagonLayer);
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
