namespace FallMonke.UI.Buttons;

public class StartGameButton : PressableButton
{
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        if (CustomGameManager.instance is not CustomGameManager manager)
            return;

        manager.BroadcastController.RequestStartGame();
    }
}
