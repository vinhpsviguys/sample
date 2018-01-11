public class EventInGame
{
    internal int event_lucky;
    public EventInGame()
    {
        event_lucky = 0;
    }
    public EventInGame(string _value)
    {
        event_lucky = int.Parse(_value);
    }
}
