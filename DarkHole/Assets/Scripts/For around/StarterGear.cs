using UnityEngine;

public class StarterGear : MonoBehaviour
{
    [Header(" Названия в инвентаре")]
    public string pickaxeName = "WoodenPickaxe";
    public string swordName = "WoodenSword";

    private void Start()
    {
        GiveStartingGear();
    }

    public void GiveStartingGear()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("❌ PlayerInventory не найден! Убедись, что он есть на сцене.");
            return;
        }

        if (WeaponManager.Instance == null)
        {
            Debug.LogError("❌ WeaponManager не найден!");
            return;
        }

        // 1. Добавляем в инвентарь
        PlayerInventory.Instance.AddItem(pickaxeName, 1);
        PlayerInventory.Instance.AddItem(swordName, 1);
        Debug.Log($"🎒 Выдано: {pickaxeName}, {swordName}");

        // 2. Экипируем кирку по умолчанию
        WeaponManager.Instance.EquipWeapon(WeaponType.Pickaxe);

        // 3. Применяем статы деревянной кирки (Урон 10, Пробитие 5)
        if (WeaponManager.Instance.playerMiner != null)
        {
            WeaponManager.Instance.playerMiner.SetStats(10f, 5f);
        }
        else
        {
            Debug.LogWarning("⚠️ В WeaponManager не назначен PlayerMiner!");
        }

        // Меч пока в инвентаре. Статы применятся автоматически при переключении.
    }
}