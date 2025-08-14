using UnityEngine;

public class DamagePopupUI : BaseUI
{
    public GameObject damagePopupPrefab;
    
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeSelf;

    public void Awake()
    {
        ObjectPoolManager.Instance.CreatePool(damagePopupPrefab.GetComponent<DamagePopup>(), 100);
    }

    public void UseDamagePopup(CharacterBaseController controller, int damageAmount, bool isEnemy, bool isCriticalHit)
    {
        DamagePopup damagePopup = ObjectPoolManager.Instance.Spawn<DamagePopup>(damagePopupPrefab.name);
        damagePopup.transform.SetParent(this.transform);
        damagePopup.OpenPopup(damagePopupPrefab.name, controller, damageAmount, isEnemy, isCriticalHit, Camera.main);
    }
    
    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
