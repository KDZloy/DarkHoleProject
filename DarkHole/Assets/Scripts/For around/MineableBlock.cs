using UnityEngine;
using System;

public class MineableBlock : MonoBehaviour
{
    [Header("⛏️ Характеристики")]
    [SerializeField] private float baseHp = 100f;
    [SerializeField] private float hardness = 50f;
    private float currentHp;
    private float initialMaxHp;

    [Header("🎨 Модели стадий (перетащи СЮДА ПРЕФАБЫ, а не объекты со сцены)")]
    [Tooltip("0: Целая | 1: Трещины | 2: Сильно повреждена")]
    [SerializeField] private GameObject[] stageModels = new GameObject[3]; 

    
    [Serializable]
    public struct OreDrop
    {
        public GameObject orePrefab;
        public float weight;
    }
    [SerializeField] private OreDrop[] oreDrops;

    [Header("⚙️ Физика выпадения")]
    [SerializeField] private Vector3 dropDirection = Vector3.right;
    [SerializeField] private float dropForce = 5f;

    private GameObject _currentModel;
    private int _currentStageIndex = -1;

    private void Start()
    {
        initialMaxHp = baseHp + hardness;
        currentHp = initialMaxHp;

        // Спавним первую модель сразу
        SwitchModel(0);
        Debug.Log($"[Глыба] Создана | HP: {currentHp}/{initialMaxHp}");
    }

    // 🎯 Вызывается киркой при столкновении
    public void TakeDamage(float damage, float penetration)
    {
        hardness = Mathf.Max(0, hardness - penetration);
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        Debug.Log($"[Глыба] ⚔️ Удар! -{damage} урона | Осталось: {currentHp}/{initialMaxHp}");

        // Определяем, какая стадия должна быть сейчас
        float percent = currentHp / initialMaxHp;
        int newStage = percent > 0.66f ? 0 : (percent > 0.33f ? 1 : 2);

        // Если стадия изменилась → удаляем старую модель, спавним новую
        if (newStage != _currentStageIndex)
        {
            SwitchModel(newStage);
        }

        if (currentHp <= 0)
            DestroyBlock();
    }

    private void SwitchModel(int stageIndex)
    {
        // 1. Удаляем предыдущую модель
        if (_currentModel != null)
            Destroy(_currentModel);

        // 2. Выбираем префаб для новой стадии
        if (stageIndex < 0 || stageIndex >= stageModels.Length || stageModels[stageIndex] == null)
        {
            Debug.LogWarning($"[Глыба] Модель для стадии {stageIndex} не назначена в инспекторе!");
            return;
        }

        // 3. Спавним новую модель как дочерний объект с нулевым смещением
        _currentModel = Instantiate(stageModels[stageIndex], transform);
        _currentModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        _currentStageIndex = stageIndex;
    }

    private void DestroyBlock()
    {
        Debug.Log($"[Глыба] 💥 Уничтожена! Выпадает руда...");
        DropOre();
        Destroy(gameObject);
    }

    private void DropOre()
    {
        if (oreDrops == null || oreDrops.Length == 0) return;

        float totalWeight = 0;
        foreach (var drop in oreDrops) totalWeight += drop.weight;

        float roll = UnityEngine.Random.Range(0, totalWeight);
        GameObject chosenPrefab = null;

        foreach (var drop in oreDrops)
        {
            roll -= drop.weight;
            if (roll <= 0)
            {
                chosenPrefab = drop.orePrefab;
                break;
            }
        }

        if (chosenPrefab == null) return;

        Vector3 spawnPos = transform.position + dropDirection.normalized * 1.2f;
        GameObject ore = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        if (ore.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(dropDirection.normalized * dropForce + Vector3.up * 2f, ForceMode.Impulse);
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * 5f);
        }
    }

    public float GetCurrentHp() => currentHp;
    public float GetMaxHp() => initialMaxHp;
}