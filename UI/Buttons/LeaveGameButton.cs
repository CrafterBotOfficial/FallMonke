namespace FallMonke.UI.Buttons;

public class LeaveGameButton : PressableButton
{
    public override void ButtonActivation()
    {
        base.ButtonActivation();
        Photon.Pun.PhotonNetwork.Disconnect(); // calls disconnect cleanup stuff
        TeleportController.TeleportToStump();
    }
}
