// this is just testing code, once the scene is ready we will load it ontop of the normal world. It will contain all of the assets and decorations and hexagons

using UnityEngine;
using System.Linq;
using FallMonke.Hexagon;

namespace FallMonke;

public static class WorldManager
{
    public static GameObject HexagonAsset;

    private const int LAYER_COUNT = 3;
    private const float LAYER_SPACING = 25f;
    private static HexagonParent[] layers;

    public static void LoadWorld()
    {
        layers = new HexagonParent[LAYER_COUNT];
        for (int i = 0; i < LAYER_COUNT; i++)
        {
            layers[i] = GameObject.Instantiate<GameObject>(HexagonAsset).GetComponent<HexagonParent>();
            layers[i].transform.localPosition += Vector3.down * (i * LAYER_SPACING);
        }
        GorillaLocomotion.GTPlayer.Instance.TeleportTo(new Vector3(210, 760, 195), Quaternion.identity, true);
    }

    public static void UnloadWorld()
    {
        Main.Log("Removing world");
        foreach (var layer in layers)
        {
            GameObject.Destroy(layer);
        }
    }

    public static int GetRemainingTiles()
    {
        int result = 0;
        foreach (var layer in layers)
        {
            foreach (var tile in layer.Hexagons)
            {
                if (!tile.IsFalling)
                {
                    result++;
                }
            }
        }
        return result;
    }
}
