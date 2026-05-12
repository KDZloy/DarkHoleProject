using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header(" Настройки")]
    [Tooltip("Название как в WeaponManager: WoodenPickaxe или WoodenSword")]
    public string weaponName = "WoodenPickaxe";
    
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private GameObject promptUI; // Текст "Нажми E" (опционально)

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        
        if (promptUI != null) promptUI.SetActive(dist <= interactRange);

        if (dist <= interactRange && Input.GetKeyDown(interactKey))
        {
            Pickup();
        }
    }

        private void Pickup()
{
    if (WeaponManager.Instance != null)
    {
        // ✅ ПРАВИЛЬНО: используем EquipByItemName вместо EquipWeapon
       WeaponManager.Instance.CraftAndEquip(weaponName);
        
        // Добавляем в инвентарь
        PlayerInventory.Instance.AddItem(weaponName, 1);
        
        Debug.Log($"✨ Подобрано: {weaponName}");
    }
    
    gameObject.SetActive(false);
}
    
}