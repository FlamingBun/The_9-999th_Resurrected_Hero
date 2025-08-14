public class StatEventData
{
    public StatType StatType { get; }
    public float PreValue { get; }
    public float CurValue { get; }
    public float EventValue { get; }

    public StatEventData(StatType statType, float preValue, float curValue, float eventValue)
    {
        StatType = statType;
        PreValue = preValue;
        CurValue = curValue;
        EventValue = eventValue;
    }
}