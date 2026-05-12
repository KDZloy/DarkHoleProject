using UnityEngine;
using System;

public class MineableBlock : MonoBehaviour
{
    [Header("⛏️ Характеристики")]
    [SerializeField] private float baseHp = 100f;
    [SerializeField] private float hardness = 50f;
    private float currentHp;
    private float initialMaxHp;

    [Header("🎨 Модели стадий")]
    [SerializeField] private GameObject[] stageModels = new GameObject[3]; 

    [Serializable]
    public struct OreDrop
    {
        public GameObject orePrefab;
        public float weight;
    }
    [SerializeField] private OreDrop[] oreDrops;

    [Header("⚙️ Физика выпадения")]
    [SerializeField] private float dropForce = 1f;
    [SerializeField] private float dropSpread = 0.3f;

    private GameObject _currentModel;
    private int _currentStageIndex = -1;

    private void Start()
    {
        initialMaxHp = baseHp + hardness;
        currentHp = initialMaxHp;
        SwitchModel(0);
    }

    // 🔹 Принимает только 2 параметра (как у тебя в PlayerMiner)
    public void TakeDamage(float damage, float penetration)
    {
        hardness = Mathf.Max(0, hardness - penetration);
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        float percent = currentHp / initialMaxHp;
        int newStage = percent > 0.66f ? 0 : (percent > 0.33f ? 1 : 2);

        if (newStage != _currentStageIndex)
            SwitchModel(newStage);

        if (currentHp <= 0)
            DestroyBlock();
    }

    private void SwitchModel(int stageIndex)
    {
        if (_currentModel != null) Destroy(_currentModel);
        if (stageIndex < 0 || stageIndex >= stageModels.Length || stageModels[stageIndex] == null) return;

        _currentModel = Instantiate(stageModels[stageIndex], transform);
        _currentModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        _currentStageIndex = stageIndex;
    }

    private void DestroyBlock()
    {
        DropOre();
        BlockRespawner respawner = GetComponent<BlockRespawner>();
        respawner?.OnBlockDestroyed();
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

        Vector3 spawnPos = transform.position + new Vector3(
            UnityEngine.Random.Range(-dropSpread, dropSpread),
            0.2f,
            UnityEngine.Random.Range(-dropSpread, dropSpread)
        );

        GameObject ore = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        if (ore.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            Vector3 force = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(0.1f, 0.3f),
                UnityEngine.Random.Range(-0.5f, 0.5f)
            );
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    // ✅ ТОЛЬКО ОДИН РАЗ! Возвращает текущее HP глыбы
    public float GetCurrentHp() => currentHp;
    public float GetMaxHp() => initialMaxHp;
}