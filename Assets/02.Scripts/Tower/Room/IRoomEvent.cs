public interface IRoomEvent
{
    public int Order { get; }
    public bool IsCleared { get; }

    public void Init(RoomController room);
    public void StartEvent();
}