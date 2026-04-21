using UnityEngine;
using UnityEngine.UI;
using TMPro; // Если используешь TextMeshPro

public class OreUIManager : MonoBehaviour
{
    [System.Serializable]
    public class OreUIData
    {
        public string oreName;        // Название руды (как в PlayerInventory)
        public Image icon;            // Иконка на UI
        public TextMeshProUGUI countText; // Текст счётчика (или Text для обычного UI)
    }

    [Header("📋 Список ресурсов на UI")]
    public OreUIData[] oreUIList; // Заполни в инспекторе

    private void Start()
    {
        // Инициализируем все счётчики нулями при старте
        UpdateAllUI();
    }

    // 🔹 Публичный метод для обновления одного ресурса
    public void UpdateOreUI(string oreName, int count)
    {
        foreach (var oreData in oreUIList)
        {
            if (oreData.oreName == oreName && oreData.countText != null)
            {
                oreData.countText.text = count.ToString();
                
                // 🎨 Анимация при изменении (опционально)
                // StartCoroutine(PulseText(oreData.countText));
                return;
            }
        }
    }

    // 🔹 Обновить всё сразу (при загрузке)
    public void UpdateAllUI()
    {
        if (PlayerInventory.Instance == null) return;

        foreach (var oreData in oreUIList)
        {
            int count = PlayerInventory.Instance.GetOreCount(oreData.oreName);
            if (oreData.countText != null)
            {
                oreData.countText.text = count.ToString();
            }
        }
    }

    // 🔹 Вызывать из PlayerInventory при изменении инвентаря
    public static OreUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}