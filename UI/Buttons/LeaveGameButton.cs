namespace FallMonke.UI.Buttons;

public class LeaveGameButton : PressableButton
{
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        NetworkSystem.Instance.ReturnToSinglePlayer();
    }
}
