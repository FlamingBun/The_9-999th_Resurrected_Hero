using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    public void FlipHealthBar(float x)
    {
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(healthPercent, 0f, 1f), 1f, 1f);
    }
}
