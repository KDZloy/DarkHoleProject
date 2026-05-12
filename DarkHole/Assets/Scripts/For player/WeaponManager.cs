using UnityEngine;

public enum WeaponType { None, Pickaxe, Sword }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("🗡️ Модели оружия (перетащи объекты-держатели)")]
    public GameObject pickaxeHolder;
    public GameObject swordHolder;

    [Header("🔧 Скрипты оружия")]
    public Pickaxe pickaxeScript;
    public SwordCombat swordScript;

    public WeaponType CurrentWeapon { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // По умолчанию экипируем кирку
        EquipWeapon(WeaponType.Pickaxe);
    }

    private void Update()
    {
        // 🔹 Переключение клавишами 1 и 2
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(WeaponType.Sword);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(WeaponType.Pickaxe);

        // 🔹 Атака по ЛКМ
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (CurrentWeapon == WeaponType.Sword)
                swordScript?.Swing();
            else if (CurrentWeapon == WeaponType.Pickaxe)
                pickaxeScript?.Swing();
        }
    }

    // 🔹 Публичный метод экипировки
    public void EquipWeapon(WeaponType type)
    {
        CurrentWeapon = type;

        // Включаем/выключаем модели
        if (pickaxeHolder) pickaxeHolder.SetActive(type == WeaponType.Pickaxe);
        if (swordHolder) swordHolder.SetActive(type == WeaponType.Sword);

        // Включаем/выключаем скрипты
        if (pickaxeScript) pickaxeScript.enabled = type == WeaponType.Pickaxe;
        if (swordScript) swordScript.enabled = type == WeaponType.Sword;

        Debug.Log($"🔫 Экипировано: {type}");
    }

    // 🔹 Авто-экипировка по названию предмета (вызывается из наковальни)
    public void EquipByItemName(string itemName)
    {
        if (itemName.Contains("Sword"))
            EquipWeapon(WeaponType.Sword);
        else if (itemName.Contains("Pickaxe"))
            EquipWeapon(WeaponType.Pickaxe);
    }
}
