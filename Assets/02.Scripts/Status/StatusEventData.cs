public readonly struct StatusEventData
{
    public StatType StatType { get; }
    public float PreValue { get; }
    public float CurValue { get; }
    public float EventValue { get; }
    public float MaxValue { get; }

    public StatusEventData(StatType statType, float preValue, float curValue, float eventValue, float maxValue)
    {
        StatType = statType;
        PreValue = preValue;
        CurValue = curValue;
        EventValue = eventValue;
        MaxValue = maxValue;
    }
}