using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetHealthBarValue(float currentHealth, float maxHealth)
    {
        healthText.text = $"{(int)currentHealth} / {(int)maxHealth}";
        
        healthBar.fillAmount = Mathf.Clamp(currentHealth/maxHealth, 0f, 1f);
    }
}
