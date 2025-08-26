using System.Linq;
using UnityEngine;

namespace FallMonke.UI.Buttons;

public class LeaveGameButton : PressableButton
{
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        NetworkSystem.Instance.ReturnToSinglePlayer();

        const string GEO_SHOW_TRIGGER_NAME = "LeavingCanyonGeo";

        var trigger = GameObject.FindObjectsOfType<GorillaGeoHideShowTrigger>().FirstOrDefault(x => x.gameObject.name == GEO_SHOW_TRIGGER_NAME);
        if (trigger) // for some reason simply referencing it is enough to completely fix the issue. No clue why.
        {
        //     trigger.OnBoxTriggered();
        //     return;
        }
        // Main.Log($"Couldn't find {GEO_SHOW_TRIGGER_NAME}, likely changed due to a game update.", BepInEx.Logging.LogLevel.Warning);
    }
}
