using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AnvilUIManager : MonoBehaviour
{
    // 🔹 Тип материала
    public enum MaterialType
    {
        Iron,
        Copper,
        Gold,
        Diamond,
        Cobalt
    }

    // 🔹 Рецепт: Слиток → Деталь
    [System.Serializable]
    public class PartRecipe
    {
        public string partName;          // Название детали (Blade, Handle, PickaxeHead)
        public MaterialType material;    // Материал детали (Iron, Copper...)
        public string requiredIngot;     // Какой слиток нужен (IronIngot, CopperIngot...)
        public int ingotsNeeded;         // Сколько слитков нужно
        public Image partIcon;           // Иконка детали
        public TextMeshProUGUI partText; // Текст "Лезвие: 3 слитка"
        public TextMeshProUGUI haveText; // Текст "У вас: 5 слитков"
        public Button craftButton;       // Кнопка "Создать"
    }

    // 🔹 Рецепт: Детали → Готовый предмет
    [System.Serializable]
    public class ItemRecipe
    {
        public string itemName;                  // Название предмета (IronSword, CopperPickaxe...)
        public MaterialType requiredMaterial;    // Требуемый материал (должен совпадать у обеих деталей!)
        public string part1Name;                 // Деталь 1 (Blade или PickaxeHead)
        public int part1Count;                   // Сколько нужно (1)
        public string part2Name;                 // Деталь 2 (Handle)
        public int part2Count;                   // Сколько нужно (1)
        public Image itemIcon;                   // Иконка предмета
        public TextMeshProUGUI itemText;         // Текст "Железный меч"
        public TextMeshProUGUI partsText;        // Текст "Нужно: Лезвие + Рукоять"
        public TextMeshProUGUI havePartsText;    // Текст "У вас: Лезвие x1, Рукоять x1"
        public TextMeshProUGUI errorText;        // Текст ошибки (если материалы не совпадают)
        public Button craftButton;               // Кнопка "Собрать"
    }

    [Header("📋 Вкладки")]
    [SerializeField] private GameObject partsTab;
    [SerializeField] private GameObject assemblyTab;
    [SerializeField] private Button tabPartsBtn;
    [SerializeField] private Button tabAssemblyBtn;

    [Header("🔧 Рецепты деталей")]
    public PartRecipe[] partRecipes;

    [Header("⚔️ Рецепты предметов")]
    public ItemRecipe[] itemRecipes;

    [Header("🔒 Закрыть UI")]
    public Button closeButton;

    private GameObject _activeTab;

    public static AnvilUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseAnvil);

        if (tabPartsBtn != null)
            tabPartsBtn.onClick.AddListener(() => SwitchTab(partsTab));
        if (tabAssemblyBtn != null)
            tabAssemblyBtn.onClick.AddListener(() => SwitchTab(assemblyTab));

        foreach (var recipe in partRecipes)
        {
            if (recipe.craftButton != null)
            {
                string partName = recipe.partName;
                recipe.craftButton.onClick.AddListener(() => CraftPart(partName));
            }
        }

        foreach (var recipe in itemRecipes)
        {
            if (recipe.craftButton != null)
            {
                string itemName = recipe.itemName;
                recipe.craftButton.onClick.AddListener(() => AssembleItem(itemName));
            }
        }

        SwitchTab(partsTab);
    }

    private void SwitchTab(GameObject newTab)
    {
        if (_activeTab != null) _activeTab.SetActive(false);
        _activeTab = newTab;
        if (_activeTab != null) _activeTab.SetActive(true);
        UpdateAllTabs();
    }

    public void UpdateAllTabs()
    {
        UpdatePartRecipes();
        UpdateItemRecipes();
    }

    private void UpdatePartRecipes()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var recipe in partRecipes)
        {
            int haveIngots = PlayerInventory.Instance.GetOreCount(recipe.requiredIngot);
            
            if (recipe.haveText != null)
                recipe.haveText.text = $"Слитков: {haveIngots}";
            if (recipe.partText != null)
                recipe.partText.text = $"{recipe.partName}: {recipe.ingotsNeeded} слитка(ов)";
            if (recipe.craftButton != null)
                recipe.craftButton.interactable = haveIngots >= recipe.ingotsNeeded;
        }
    }

    private void UpdateItemRecipes()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var recipe in itemRecipes)
        {
            int havePart1 = PlayerInventory.Instance.GetOreCount(recipe.part1Name);
            int havePart2 = PlayerInventory.Instance.GetOreCount(recipe.part2Name);
            
            // 🔹 ПРОВЕРКА: Материалы должны совпадать!
            bool materialsMatch = CheckMaterialsMatch(recipe.part1Name, recipe.part2Name, recipe.requiredMaterial);
            bool hasEnoughParts = havePart1 >= recipe.part1Count && havePart2 >= recipe.part2Count;
            bool canCraft = materialsMatch && hasEnoughParts;
            
            if (recipe.partsText != null)
                recipe.partsText.text = $"Нужно: {recipe.part1Name}×{recipe.part1Count} + {recipe.part2Name}×{recipe.part2Count}";
            
            if (recipe.havePartsText != null)
                recipe.havePartsText.text = $"У вас: {recipe.part1Name}×{havePart1}, {recipe.part2Name}×{havePart2}";
            
            // 🔹 Показываем ошибку если материалы не совпадают
            if (recipe.errorText != null)
            {
                if (!materialsMatch && hasEnoughParts)
                {
                    recipe.errorText.text = "❌ Материалы не совпадают!";
                    recipe.errorText.color = Color.red;
                }
                else
                {
                    recipe.errorText.text = "";
                }
            }
            
            if (recipe.craftButton != null)
                recipe.craftButton.interactable = canCraft;
        }
    }

    // 🔹 Проверка совпадения материалов
    private bool CheckMaterialsMatch(string part1Name, string part2Name, MaterialType requiredMaterial)
    {
        // Находим материалы обеих деталей в рецептах
        PartRecipe part1Recipe = Array.Find(partRecipes, r => r.partName == part1Name && r.material == requiredMaterial);
        PartRecipe part2Recipe = Array.Find(partRecipes, r => r.partName == part2Name && r.material == requiredMaterial);
        
        // Обе детали должны существовать с нужным материалом
        return part1Recipe != null && part2Recipe != null;
    }

    // 🔹 ЭТАП 1: Создать деталь из слитков
    public void CraftPart(string partName)
    {
        var recipe = Array.Find(partRecipes, r => r.partName == partName);
        if (recipe == null) return;

        int haveIngots = PlayerInventory.Instance.GetOreCount(recipe.requiredIngot);
        if (haveIngots >= recipe.ingotsNeeded)
        {
            PlayerInventory.Instance.RemoveOre(recipe.requiredIngot, recipe.ingotsNeeded);
            PlayerInventory.Instance.AddPart(recipe.partName, 1);
            
            Debug.Log($"🔨 Создана деталь: {recipe.partName} ({recipe.material})");
            UpdateAllTabs();
        }
    }

    // 🔹 ЭТАП 2: Собрать предмет из деталей (ПРОВЕРКА МАТЕРИАЛОВ!)
    public void AssembleItem(string itemName)
    {
        var recipe = Array.Find(itemRecipes, r => r.itemName == itemName);
        if (recipe == null) return;

        int havePart1 = PlayerInventory.Instance.GetOreCount(recipe.part1Name);
        int havePart2 = PlayerInventory.Instance.GetOreCount(recipe.part2Name);

        // 🔹 Проверка материалов
        if (!CheckMaterialsMatch(recipe.part1Name, recipe.part2Name, recipe.requiredMaterial))
        {
            Debug.LogError($"❌ ОШИБКА: Материалы не совпадают для {recipe.itemName}!");
            return;
        }

        if (havePart1 >= recipe.part1Count && havePart2 >= recipe.part2Count)
        {
            PlayerInventory.Instance.RemovePart(recipe.part1Name, recipe.part1Count);
            PlayerInventory.Instance.RemovePart(recipe.part2Name, recipe.part2Count);
            PlayerInventory.Instance.AddItem(recipe.itemName, 1);
            
            Debug.Log($"⚔️ Собран предмет: {recipe.itemName} ({recipe.requiredMaterial})");
            UpdateAllTabs();
            WeaponManager.Instance?.EquipByItemName(recipe.itemName);
        }
    }

    public void CloseAnvil()
    {
        gameObject.SetActive(false);
    }
}