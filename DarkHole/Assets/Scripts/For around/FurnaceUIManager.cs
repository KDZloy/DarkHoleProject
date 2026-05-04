using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FurnaceUIManager : MonoBehaviour
{
    [System.Serializable]
    public class SmeltRecipe
    {
        public string oreName;
        public int oreRequired;
        public string ingotName;
        public Image oreIcon;
        public TextMeshProUGUI oreText;
        public TextMeshProUGUI haveText;
        public Button craftButton;
    }

    [Header("📋 Рецепты")]
    public SmeltRecipe[] recipes;

    [Header("🔒 Закрыть UI")]
    public Button closeButton;

    public static FurnaceUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseFurnace);

        foreach (var recipe in recipes)
        {
            if (recipe.craftButton != null)
            {
                string oreName = recipe.oreName;
                recipe.craftButton.onClick.AddListener(() => CraftIngot(oreName));
            }
        }
    }

    // 🔹 OnEnable/OnDisable больше не нужны — CursorManager делает всё сам!
    // Но можно оставить для обновления рецептов:
    private void OnEnable()
    {
        UpdateRecipes();
    }

    public void UpdateRecipes()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var recipe in recipes)
        {
            int haveOre = PlayerInventory.Instance.GetOreCount(recipe.oreName);
            
            if (recipe.haveText != null)
                recipe.haveText.text = $"У вас: {haveOre}";
            
            if (recipe.oreText != null)
                recipe.oreText.text = $"{recipe.oreName}: {recipe.oreRequired}";

            if (recipe.craftButton != null)
                recipe.craftButton.interactable = haveOre >= recipe.oreRequired;
        }
    }

    public void CraftIngot(string oreName)
    {
        if (PlayerInventory.Instance == null) return;

        SmeltRecipe recipe = System.Array.Find(recipes, r => r.oreName == oreName);
        if (recipe == null) return;

        int haveOre = PlayerInventory.Instance.GetOreCount(oreName);

        if (haveOre >= recipe.oreRequired)
        {
            PlayerInventory.Instance.RemoveOre(oreName, recipe.oreRequired);
            PlayerInventory.Instance.AddOre(recipe.ingotName, 1);
            
            Debug.Log($"🔥 Переплавлено: {recipe.oreRequired} {oreName} → 1 {recipe.ingotName}");
            
            UpdateRecipes();
            
            if (OreUIManager.Instance != null)
            {
                OreUIManager.Instance.UpdateAllUI();
            }
        }
        else
        {
            Debug.LogWarning("❌ Недостаточно руды!");
        }
    }

    public void CloseFurnace()
    {
        gameObject.SetActive(false);
        // Курсор скроется автоматически через CursorManager.CloseUI()
    }
}