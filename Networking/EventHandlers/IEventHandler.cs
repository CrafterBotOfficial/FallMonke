namespace FallMonke.Networking.EventHandlers;

public interface IEventHandler
{
    public void OnEvent(NetPlayer sender, object data);
}
