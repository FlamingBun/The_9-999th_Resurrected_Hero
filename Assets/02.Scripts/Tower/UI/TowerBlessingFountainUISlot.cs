using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerBlessingFountainUISlot : MonoBehaviour
{
    public List<StatModifier> Modifiers => _modifiers;
    
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button button;

    [SerializeField] private Color lowCloColor;
    [SerializeField] private Color mediumColor;
    [SerializeField] private Color highColor;
    

    private List<StatModifier> _modifiers;
    
    public void Init(TowerBlessingFountainUI rootUI)
    {
        button.onClick.AddListener(() => rootUI.OnSelectBlessSlot(this));
    }

    public void SetInfo(List<BlessingFountainBlessInstance> blessInstances)
    {
        _modifiers = new();
        infoText.text = "";

        string typeText = "";
        
        foreach (var bless in blessInstances)
        {

            if (bless.mod.statType.ToString() != typeText)
            {
                typeText = bless.mod.statType.ToString();
                infoText.text += "\n";
            }
            
            _modifiers.Add(bless.mod);

            string colorTag = GetColorTage(bless.grade);

            if (bless.modType == StatModType.Flat)
            {
                infoText.text += $"<color=#{colorTag}>{bless.blessName} + {bless.mod.value}{bless.addText}<color=#{colorTag}>\n";
            }
            else
            {
                infoText.text += $"<color=#{colorTag}>{bless.blessName} + {bless.mod.value * 100}{bless.addText}<color=#{colorTag}>\n";
            }
        }
    }

    private string GetColorTage(BlessingFountainBlessInstance.Grade grade)
    {
        return grade switch
        {
            BlessingFountainBlessInstance.Grade.Low => ColorUtility.ToHtmlStringRGB(lowCloColor),
            BlessingFountainBlessInstance.Grade.Medium => ColorUtility.ToHtmlStringRGB(mediumColor),
            BlessingFountainBlessInstance.Grade.High => ColorUtility.ToHtmlStringRGB(highColor),
            _ => ""
        };
    }
}