using FallMonke.Hexagon;
using UnityEngine;

namespace FallMonke;

public class LocalPlayerManager : ParticipantManager
{
    private LayerMask hexagonLayer = LayerMask.GetMask("Gorilla Object");
    private Transform[] rigHands;

    private bool reportDeathSent;

    private void Start()
    {
        if (!Info.Player.IsLocal)
        {
            Main.Log("Being added to wrong player!", BepInEx.Logging.LogLevel.Warning);
            DestroyImmediate(this);
            return;
        }
        rigHands = [
            Rig.leftHandTransform,
            Rig.rightHandTransform
        ];
    }

    // the idea behind this is everything will happen twice, once on locally only for the player and another for the master client
    private void Update()
    {
        var manager = (CustomGameManager)GorillaGameManager.instance; // https://www.youtube.com/shorts/pFB5F-fS_Y4

        if (Info.IsDead)
            return;

        if (transform.position.y < WorldManager.Instance.EliminationHeight && !reportDeathSent)
        {
            Main.Log("I've been eliminated");
            reportDeathSent = true;
            manager.BroadcastController.SendReportElimination();
            TeleportController.TeleportToLobby();
            return;
        }

        // change: Master no longer manages tiles, each player does then tells everyone else when fell
        if (Info.Player.IsLocal && !manager.CooldownInAffect)
        {
            FallableHexagon hexagon;

            // body down raycast
            if (TryRaycastToTerrain(Rig.transform.position, 1f, out hexagon))
            {
                OnTouchHexagon(manager, hexagon);
                return;
            }

            // hands - only if nothing under body
            foreach (var hand in rigHands)
                if (TryRaycastToTerrain(hand.position, .1f, out hexagon))
                {
                    OnTouchHexagon(manager, hexagon);
                }
        }
    }

    private void OnTouchHexagon(CustomGameManager manager, FallableHexagon hexagon)
    {
        if (!hexagon.IsFalling)
        {
            Main.Log("Yeeting platform", BepInEx.Logging.LogLevel.Debug);
            hexagon.Fall();
            manager.BroadcastController.FallPlatform(hexagon);
        }
    }

    private bool TryRaycastToTerrain(Vector3 origin, float distance, out FallableHexagon hitPlatform)
    {
        const float radius = 0.1f;

        bool hitSomething = Physics.SphereCast(origin + new Vector3(0f, .025f, 0f), radius, Vector3.down, out RaycastHit hit, distance, hexagonLayer);
        if (hitSomething && hit.transform.gameObject.TryGetComponent(out hitPlatform))
        {
            return true;
        }

        hitPlatform = default;
        return false;
    }

    // handle player leave
    private void OnDisable()
    {
        Info.IsDead = true;
        Destroy(this);
    }
}
