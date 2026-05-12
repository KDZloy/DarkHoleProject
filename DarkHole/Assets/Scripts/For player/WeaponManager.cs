using UnityEngine;

public enum WeaponType { None, Pickaxe, Sword }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("🗡️ Модели оружия (держатели)")]
    public GameObject pickaxeHolder;
    public GameObject swordHolder;

    [Header("🔧 Скрипты оружия")]
    public PlayerMiner playerMiner;
    public SwordCombat swordScript;

    public WeaponType CurrentWeapon { get; private set; }
    private string _currentEquippedName = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        EquipWeapon(WeaponType.Pickaxe);
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(WeaponType.Sword);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(WeaponType.Pickaxe);

        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (CurrentWeapon == WeaponType.Sword) swordScript?.Swing();
            else if (CurrentWeapon == WeaponType.Pickaxe) playerMiner?.Swing();
        }
    }

    public void EquipWeapon(WeaponType type)
    {
        CurrentWeapon = type;
        if (pickaxeHolder) pickaxeHolder.SetActive(type == WeaponType.Pickaxe);
        if (swordHolder) swordHolder.SetActive(type == WeaponType.Sword);
        Debug.Log($"🔫 Экипировано: {type}");
    }

    public void CraftAndEquip(string newItemName)
    {
        WeaponInfo info = GetWeaponInfo(newItemName);

        if (!string.IsNullOrEmpty(_currentEquippedName))
        {
            WeaponInfo oldInfo = GetWeaponInfo(_currentEquippedName);
            if (oldInfo.Type == info.Type)
            {
                PlayerInventory.Instance.RemoveItem(_currentEquippedName);
                Debug.Log($"🗑️ Удалено старое: {_currentEquippedName}");
            }
        }

        PlayerInventory.Instance.AddItem(newItemName, 1);
        _currentEquippedName = newItemName;
        EquipWeapon(info.Type);
        ApplyStats(info.Damage, info.Type);
        
        Debug.Log($"⚔️ Крафт завершен: {_currentEquippedName} экипировано! Урон: {info.Damage}");
    }

    private struct WeaponInfo
    {
        public WeaponType Type;
        public float Damage;      // 🔹 float, а не int
        public string Material;
    }

    private WeaponInfo GetWeaponInfo(string itemName)
    {
        WeaponInfo info = new WeaponInfo();
        
        if (itemName.EndsWith("P"))
        {
            info.Type = WeaponType.Pickaxe;
            info.Material = itemName.Substring(0, itemName.Length - 1);
        }
        else
        {
            info.Type = WeaponType.Sword;
            info.Material = itemName;
        }

        // Урон по материалам (твои значения)
        switch (info.Material)
        {
            case "Wood":    info.Damage = 10f; break;
            case "Copper":  info.Damage = 20f; break;
            case "Iron":    info.Damage = 25f; break;
            case "Gold":    info.Damage = 30f; break;
            case "Diamond": info.Damage = 40f; break;
            case "Cobalt":  info.Damage = 50f; break;
            default:        info.Damage = 10f; break;
        }
        return info;
    }

    // 🔹 ИСПРАВЛЕННЫЙ МЕТОД: теперь переменные объявлены правильно
    private void ApplyStats(float damageValue, WeaponType type)
    {
        CurrentMaterial = ParseMaterial(_currentEquippedName); // ✅ ЗАПОМИНАЕМ МАТЕРИАЛ
        float penetrationValue = damageValue * 0.6f; // Пробитие = 60% от урона

        if (CurrentWeapon == WeaponType.Pickaxe && playerMiner != null)
        {
            playerMiner.SetStats(damageValue, penetrationValue); // ✅ Теперь работает!
        }
        else if (CurrentWeapon == WeaponType.Sword && swordScript != null)
        {
            swordScript.SetDamage(damageValue); // ✅ Теперь работает!
        }
    }
        // 🔹 Добавь в начало класса (после CurrentWeapon)
    public string CurrentMaterial { get; private set; } = "Wood";

    // 🔹 Добавь в конец класса (перед последней })
    // Определяет материал по названию предмета ("CopperP" -> "Copper")
    private string ParseMaterial(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return "Wood";
        
        string cleanName = itemName.EndsWith("P") ? itemName.Substring(0, itemName.Length - 1) : itemName;
        
        if (cleanName.Contains("Cobalt")) return "Cobalt";
        if (cleanName.Contains("Diamond")) return "Diamond";
        if (cleanName.Contains("Gold")) return "Gold";
        if (cleanName.Contains("Iron")) return "Iron";
        if (cleanName.Contains("Copper")) return "Copper";
        if (cleanName.Contains("Wood")) return "Wood";
        
        return "Wood";
    }

    public void EquipByName(string itemName)
    {
        CraftAndEquip(itemName);
    }
}