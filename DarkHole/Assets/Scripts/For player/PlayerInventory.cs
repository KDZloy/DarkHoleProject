using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // 🔹 Синглтон для удобного доступа из любого скрипта
    public static PlayerInventory Instance { get; private set; }

    [Header("📊 Счётчик руды")]
    // Ключ: название руды, Значение: количество
    private Dictionary<string, int> _oreCounts = new Dictionary<string, int>();

    private void Awake()
    {
        // 🔹 Инициализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Опционально: не удалять при смене сцены
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("🎒 Инвентарь инициализирован");
    }

    // 🔹 Добавить руду в инвентарь
    public void AddOre(string oreName, int amount = 1)
    {
        if (_oreCounts.ContainsKey(oreName))
        {
            _oreCounts[oreName] += amount;
        }
        else
        {
            _oreCounts[oreName] = amount;
        }

        // 🔹 Вывод в консоль (как ты просил)
        Debug.Log($"📦 {oreName}: {_oreCounts[oreName]} шт.");
        
        // 🔹 Можно добавить звук подбора:
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);
    }

    // 🔹 Получить количество конкретной руды (для будущего UI)
    public int GetOreCount(string oreName)
    {
        return _oreCounts.TryGetValue(oreName, out int count) ? count : 0;
    }

    // 🔹 Вывести весь инвентарь в консоль (для отладки)
    public void PrintInventory()
    {
        Debug.Log("🎒 === ИНВЕНТАРЬ ===");
        foreach (var kvp in _oreCounts)
        {
            Debug.Log($"   {kvp.Key}: {kvp.Value}");
        }
        Debug.Log("🎒 =================");
    }
}