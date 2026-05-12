using UnityEngine;
using System.Collections.Generic;

public enum WeaponType { None, Pickaxe, Sword }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("🗡️ Модели оружия (держатели)")]
    public GameObject pickaxeHolder;
    public GameObject swordHolder;

    [Header("🔧 Скрипты оружия")]
    public PlayerMiner playerMiner;
    public SwordCombat swordCombat;

    public WeaponType CurrentWeaponType { get; private set; }
    private string _currentEquippedName = ""; // Название текущего предмета (например "CopperPickaxe")

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // По умолчанию экипируем кирку (или оставь None, если хочешь начинать без оружия)
        EquipWeapon(WeaponType.Pickaxe);
    }

    private void Update()
    {
        // Управление для тестов
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(WeaponType.Sword);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(WeaponType.Pickaxe);

        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (CurrentWeaponType == WeaponType.Sword && swordCombat != null)
                swordCombat.Swing();
            else if (CurrentWeaponType == WeaponType.Pickaxe && playerMiner != null)
                playerMiner.Swing();
        }
    }

    // 🔹 Переключение типа (Меч/Кирка)
    public void EquipWeapon(WeaponType type)
    {
        CurrentWeaponType = type;
        if (pickaxeHolder) pickaxeHolder.SetActive(type == WeaponType.Pickaxe);
        if (swordHolder) swordHolder.SetActive(type == WeaponType.Sword);
        
        // Включаем скрипты
        if (playerMiner) playerMiner.enabled = type == WeaponType.Pickaxe;
        if (swordCombat) swordCombat.enabled = type == WeaponType.Sword;
    }

    // 🔹 ГЛАВНЫЙ МЕТОД: Крафт и смена оружия
    // Вызывается из AnvilUIManager
    public void CraftAndEquip(string newItemName)
    {
        // 1. Определяем тип нового предмета
        WeaponType newType = newItemName.Contains("Sword") ? WeaponType.Sword : WeaponType.Pickaxe;

        // 2. Если уже что-то экипировано того же типа — удаляем старое
        // (Например, если была WoodenPickaxe, и мы крафтим CopperPickaxe)
        if (!string.IsNullOrEmpty(_currentEquippedName) && CurrentWeaponType == newType)
        {
            PlayerInventory.Instance.RemoveItem(_currentEquippedName);
            Debug.Log($"🗑️ Удалено старое: {_currentEquippedName}");
        }

        // 3. Добавляем новый предмет в инвентарь
        PlayerInventory.Instance.AddItem(newItemName, 1);
        _currentEquippedName = newItemName;

        // 4. Экипируем и применяем статы
        EquipWeapon(newType);
        ApplyStats(newItemName);

        Debug.Log($"⚔️ Экипировано и настроено: {newItemName}");
    }

    // 🔹 База характеристик (ЗАПОЛНИ ТУТ СВОИ ЦИФРЫ)
    private void ApplyStats(string itemName)
    {
        float damage = 10;
        float penetration = 0;
        int durability = 100;

        // --- КИРКИ ---
        if (itemName == "WoodenPickaxe") { damage = 10; penetration = 5; durability = 50; }
        else if (itemName == "StonePickaxe") { damage = 15; penetration = 10; durability = 100; }
        else if (itemName == "CopperPickaxe") { damage = 25; penetration = 15; durability = 200; }
        else if (itemName == "IronPickaxe") { damage = 35; penetration = 25; durability = 350; }
        else if (itemName == "DiamondPickaxe") { damage = 50; penetration = 40; durability = 1000; }

        // --- МЕЧИ ---
        else if (itemName == "WoodenSword") { damage = 15; penetration = 0; durability = 50; }
        else if (itemName == "StoneSword") { damage = 20; penetration = 5; durability = 100; }
        else if (itemName == "CopperSword") { damage = 30; penetration = 10; durability = 200; }
        else if (itemName == "DiamondSword") { damage = 60; penetration = 30; durability = 1000; }

        // Применяем статы к скриптам
        if (CurrentWeaponType == WeaponType.Pickaxe && playerMiner != null)
        {
            playerMiner.SetStats(damage, penetration, durability);
        }
        else if (CurrentWeaponType == WeaponType.Sword && swordCombat != null)
        {
            swordCombat.SetStats(damage, durability);
        }
    }
}