using UnityEngine;

namespace FallMonke.UI.Buttons;

public class StreamerModeButton : PressableButton
{
    public static bool MuteNotifications;

    private Animator animator;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void ButtonActivation()
    {
        base.ButtonActivation();
        if (CustomGameManager.instance is CustomGameManager manager)
        {
            animator.Play("StreamerButton", -1, 0f);
            if (MuteNotifications)
            {
                MuteNotifications = false;
                manager.NotificationHandler.ShowNotification("Notifications enabled");
            }
            else
            {
                manager.NotificationHandler.ShowNotification("Notifications disabled");
                MuteNotifications = true;
            }
        }
    }
}
