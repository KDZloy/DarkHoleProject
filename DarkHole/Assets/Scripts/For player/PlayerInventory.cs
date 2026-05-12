using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("📊 Счётчик ресурсов")]
    private Dictionary<string, int> _oreCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("🎒 Инвентарь инициализирован");
    }

    // 🔹 Добавить руду/слиток
    public void AddOre(string oreName, int amount = 1)
    {
        if (_oreCounts.ContainsKey(oreName))
            _oreCounts[oreName] += amount;
        else
            _oreCounts[oreName] = amount;

        Debug.Log($"📦 {oreName}: {_oreCounts[oreName]} шт.");
        
        if (OreUIManager.Instance != null)
        OreUIManager.Instance.UpdateResourceUI(oreName, GetOreCount(oreName)); // 🔹 ИЗМЕНЕНО
    }

    // 🔹 Добавить деталь
    public void AddPart(string partName, int amount = 1)
    {
        if (_oreCounts.ContainsKey(partName))
            _oreCounts[partName] += amount;
        else
            _oreCounts[partName] = amount;

        Debug.Log($"🔧 Деталь добавлена: {partName} x{amount}");
        
        if (OreUIManager.Instance != null)
            OreUIManager.Instance.UpdateAllUI();
    }

    // 🔹 Добавить готовый предмет
    public void AddItem(string itemName, int amount = 1)
    {
        if (_oreCounts.ContainsKey(itemName))
            _oreCounts[itemName] += amount;
        else
            _oreCounts[itemName] = amount;

        Debug.Log($"⚔️ Предмет создан: {itemName} x{amount}");
        
        if (OreUIManager.Instance != null)
            OreUIManager.Instance.UpdateAllUI();
    }

    // 🔹 Удалить руду/слиток
    public void RemoveOre(string oreName, int amount)
    {
        if (_oreCounts.ContainsKey(oreName))
        {
            _oreCounts[oreName] -= amount;
            if (_oreCounts[oreName] <= 0)
                _oreCounts.Remove(oreName);
            
            Debug.Log($"📤 Удалено: {oreName} x{amount}");
            
            if (OreUIManager.Instance != null)
        OreUIManager.Instance.UpdateResourceUI(oreName, GetOreCount(oreName)); // 🔹 ИЗМЕНЕНО
        }
    }

    // 🔹 Удалить деталь
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

    // 🔹 Получить количество
    public int GetOreCount(string oreName)
    {
        return _oreCounts.TryGetValue(oreName, out int count) ? count : 0;
    }

    // 🔹 Проверить наличие деталей
    public bool HasParts(string part1Name, int part1Count, string part2Name, int part2Count)
    {
        bool hasPart1 = GetOreCount(part1Name) >= part1Count;
        bool hasPart2 = GetOreCount(part2Name) >= part2Count;
        return hasPart1 && hasPart2;
    }

    // 🔹 Вывести весь инвентарь (для отладки)
    public void PrintInventory()
    {
        Debug.Log("🎒 === ИНВЕНТАРЬ ===");
        foreach (var kvp in _oreCounts)
            Debug.Log($"   {kvp.Key}: {kvp.Value}");
        Debug.Log("🎒 =================");
    }
        // 🔹 Добавить в PlayerInventory.cs
    public void RemoveItem(string itemName)
    {
        if (_oreCounts.ContainsKey(itemName))
        {
            _oreCounts.Remove(itemName);
            Debug.Log($"🗑️ Предмет удален: {itemName}");
            if (OreUIManager.Instance != null)
                OreUIManager.Instance.UpdateAllUI();
        }
    }
}