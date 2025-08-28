namespace FallMonke.Networking;

public struct CustomEventData(int code, object data)
{
    public int Code = code;
    public object Data = data;
}
