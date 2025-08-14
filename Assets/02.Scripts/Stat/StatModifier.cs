
public enum StatModType
{
    Flat = 100,        // 고정 수치 증가 (예: +5)
    PercentAdd = 200,  // 누적 퍼센트 증가 (예: +10%, +15% → +25%로 한번에)
    PercentMult = 300, // 곱연산 퍼센트 (예: x1.1, x1.2 차례대로 곱함)
}

public class StatModifier
{
    public StatType statType;
    public StatModType modType;  
    public float value;        
    public int order;         
    public object source;      
}
