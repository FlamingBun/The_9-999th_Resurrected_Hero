using System.Collections.Generic;

public class BlessingFountainBlessInstance
{
    public enum Grade
    {
        High,
        Medium,
        Low,
    }

    public Grade grade;
    public StatModType modType;
    public string blessName;
    public string addText;
    public StatModifier mod;

    public BlessingFountainBlessInstance(Grade grade, StatModType modType, string blessName, string addText, StatModifier mod)
    {
        this.modType = modType;
        this.grade = grade;
        this.blessName = blessName;
        this.mod = mod;
        this.addText = addText;
    }
}