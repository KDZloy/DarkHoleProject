using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnvilUIManager : MonoBehaviour
{
    // 🔹 Рецепт: Слиток → Деталь
    [System.Serializable]
    public class PartRecipe
    {
        public string partName;          // Название детали (Blade, Handle, PickaxeHead)
        public string requiredIngot;     // Какой слиток нужен (IronIngot, GoldIngot...)
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
        public string itemName;                  // Название предмета (IronSword, IronPickaxe)
        public string part1Name;                 // Деталь 1 (Blade или PickaxeHead)
        public int part1Count;                   // Сколько нужно (обычно 1)
        public string part2Name;                 // Деталь 2 (Handle - универсальная)
        public int part2Count;                   // Сколько нужно (обычно 1)
        public Image itemIcon;                   // Иконка предмета
        public TextMeshProUGUI itemText;         // Текст "Железный меч"
        public TextMeshProUGUI partsText;        // Текст "Нужно: Лезвие + Рукоять"
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
        // Кнопка закрытия
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseAnvil);

        // Переключение вкладок
        if (tabPartsBtn != null)
            tabPartsBtn.onClick.AddListener(() => SwitchTab(partsTab));
        if (tabAssemblyBtn != null)
            tabAssemblyBtn.onClick.AddListener(() => SwitchTab(assemblyTab));

        // Настраиваем кнопки рецептов деталей
        foreach (var recipe in partRecipes)
        {
            if (recipe.craftButton != null)
            {
                string partName = recipe.partName;
                recipe.craftButton.onClick.AddListener(() => CraftPart(partName));
            }
        }

        // Настраиваем кнопки рецептов предметов
        foreach (var recipe in itemRecipes)
        {
            if (recipe.craftButton != null)
            {
                string itemName = recipe.itemName;
                recipe.craftButton.onClick.AddListener(() => AssembleItem(itemName));
            }
        }

        // По умолчанию открываем вкладку деталей
        SwitchTab(partsTab);
    }

    // 🔹 Переключение вкладок
    private void SwitchTab(GameObject newTab)
    {
        if (_activeTab != null) _activeTab.SetActive(false);
        _activeTab = newTab;
        if (_activeTab != null) _activeTab.SetActive(true);
        UpdateAllTabs();
    }

    // 🔹 Обновить все рецепты
    public void UpdateAllTabs()
    {
        UpdatePartRecipes();
        UpdateItemRecipes();
    }

    // 🔹 Обновить доступность рецептов деталей
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

    // 🔹 Обновить доступность рецептов предметов
    private void UpdateItemRecipes()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var recipe in itemRecipes)
        {
            bool hasPart1 = PlayerInventory.Instance.GetOreCount(recipe.part1Name) >= recipe.part1Count;
            bool hasPart2 = PlayerInventory.Instance.GetOreCount(recipe.part2Name) >= recipe.part2Count;
            bool canCraft = hasPart1 && hasPart2;
            
            if (recipe.partsText != null)
                recipe.partsText.text = $"Нужно: {recipe.part1Name}×{recipe.part1Count} + {recipe.part2Name}×{recipe.part2Count}";
            if (recipe.craftButton != null)
                recipe.craftButton.interactable = canCraft;
        }
    }

    // 🔹 Создать деталь
    public void CraftPart(string partName)
    {
        var recipe = System.Array.Find(partRecipes, r => r.partName == partName);
        if (recipe == null) return;

        int haveIngots = PlayerInventory.Instance.GetOreCount(recipe.requiredIngot);
        if (haveIngots >= recipe.ingotsNeeded)
        {
            PlayerInventory.Instance.RemoveOre(recipe.requiredIngot, recipe.ingotsNeeded);
            PlayerInventory.Instance.AddPart(recipe.partName, 1);
            
            Debug.Log($"🔨 Создана деталь: {recipe.partName}");
            UpdateAllTabs();
        }
    }

    // 🔹 Собрать предмет
    public void AssembleItem(string itemName)
    {
        var recipe = System.Array.Find(itemRecipes, r => r.itemName == itemName);
        if (recipe == null) return;

        bool hasPart1 = PlayerInventory.Instance.GetOreCount(recipe.part1Name) >= recipe.part1Count;
        bool hasPart2 = PlayerInventory.Instance.GetOreCount(recipe.part2Name) >= recipe.part2Count;

        if (hasPart1 && hasPart2)
        {
            PlayerInventory.Instance.RemovePart(recipe.part1Name, recipe.part1Count);
            PlayerInventory.Instance.RemovePart(recipe.part2Name, recipe.part2Count);
            PlayerInventory.Instance.AddItem(recipe.itemName, 1);
            
            Debug.Log($"⚔️ Собран предмет: {recipe.itemName}");
            UpdateAllTabs();
        }
    }

    public void CloseAnvil()
    {
        gameObject.SetActive(false);
    }
}