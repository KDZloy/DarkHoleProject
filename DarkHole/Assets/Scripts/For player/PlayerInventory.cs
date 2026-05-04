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

        if (OreUIManager.Instance != null)
        {
            OreUIManager.Instance.UpdateOreUI(oreName, _oreCounts[oreName]);
        }
        
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
    // Добавь этот метод в PlayerInventory.cs:

// 🔹 Удалить руду из инвентаря (для плавильни)
public void RemoveOre(string oreName, int amount)
{
    if (_oreCounts.ContainsKey(oreName))
    {
        _oreCounts[oreName] -= amount;
        
        if (_oreCounts[oreName] <= 0)
        {
            _oreCounts.Remove(oreName);
        }
        
        Debug.Log($"📤 Удалено: {oreName} x{amount}");
        
        // 🔹 Уведомляем UI
        if (OreUIManager.Instance != null)
        {
            OreUIManager.Instance.UpdateOreUI(oreName, GetOreCount(oreName));
        }
    }
}
// 🔹 Добавить деталь (для крафта на наковальне)
public void AddPart(string partName, int amount = 1)
{
    if (_oreCounts.ContainsKey(partName))
    {
        _oreCounts[partName] += amount;
    }
    else
    {
        _oreCounts[partName] = amount;
    }
    Debug.Log($"🔧 Деталь добавлена: {partName} x{amount}");
    OreUIManager.Instance?.UpdateAllUI();
}

// 🔹 Добавить готовый предмет (оружие/инструмент)
public void AddItem(string itemName, int amount = 1)
{
    if (_oreCounts.ContainsKey(itemName))
    {
        _oreCounts[itemName] += amount;
    }
    else
    {
        _oreCounts[itemName] = amount;
    }
    Debug.Log($"⚔️ Предмет создан: {itemName} x{amount}");
    OreUIManager.Instance?.UpdateAllUI();
}

// 🔹 Удалить деталь (для крафта)
public void RemovePart(string partName, int amount)
{
    if (_oreCounts.ContainsKey(partName))
    {
        _oreCounts[partName] -= amount;
        if (_oreCounts[partName] <= 0)
            _oreCounts.Remove(partName);
        Debug.Log($"🔧 Деталь использована: {partName} x{amount}");
    }
}
    
}