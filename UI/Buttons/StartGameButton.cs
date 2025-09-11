using UnityEngine;

namespace FallMonke.UI.Buttons;

public class StartGameButton : PressableButton
{
    private Animator animator;

    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void ButtonActivation()
    {
        base.ButtonActivation();
        if (CustomGameManager.instance is not CustomGameManager manager)
            return;

        animator.Play("ButtonAnimation", -1, 0f);

        if (manager.NetworkController.PlayersWithModCount() < CustomGameManager.REQUIRED_PLAYER_COUNT)
        {
            manager.NotificationHandler.ShowNotification("Not enough players");
            return;
        }
        manager.NetworkController.RequestStartGame();
    }
}
