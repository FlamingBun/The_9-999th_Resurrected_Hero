using UnityEngine;
using UnityEngine.UI;

public class TowerGemEffectHUDUI : MonoBehaviour
{
    [SerializeField] private Image slotPrefab;

    public void AddEffectImage(Sprite icon)
    {
        Instantiate(slotPrefab, transform).sprite = icon;
    }

    public void ClearEffects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
}
