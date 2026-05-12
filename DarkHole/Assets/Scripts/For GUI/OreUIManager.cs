using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OreUIManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceUIData
    {
        public string resourceName;    // Название (как в PlayerInventory)
        public Image icon;             // Иконка
        public TextMeshProUGUI countText; // Текст количества
    }

    [Header("🪨 Руда (слева)")]
    public ResourceUIData[] oreUIList;

    [Header("🔩 Слитки (справа)")]
    public ResourceUIData[] ingotUIList; // 🔹 НОВОЕ

    public static OreUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        UpdateAllUI();
    }

    // 🔹 Универсальный метод: обновляет руду ИЛИ слиток
    public void UpdateResourceUI(string resourceName, int count)
    {
        // Ищем в руде
        foreach (var data in oreUIList)
        {
            if (data.resourceName == resourceName && data.countText != null)
            {
                data.countText.text = count.ToString();
                return;
            }
        }
        // Ищем в слитках
        foreach (var data in ingotUIList)
        {
            if (data.resourceName == resourceName && data.countText != null)
            {
                data.countText.text = count.ToString();
                return;
            }
        }
    }

    // 🔹 Обновить всё сразу (при загрузке или крафте)
    public void UpdateAllUI()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var data in oreUIList)
        {
            int count = PlayerInventory.Instance.GetOreCount(data.resourceName);
            if (data.countText != null) data.countText.text = count.ToString();
        }

        foreach (var data in ingotUIList)
        {
            int count = PlayerInventory.Instance.GetOreCount(data.resourceName);
            if (data.countText != null) data.countText.text = count.ToString();
        }
    }
}