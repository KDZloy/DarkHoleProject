using UnityEngine;
using System.Collections;

public class BlockRespawner : MonoBehaviour
{
    [Header("⛏️ Настройки")]
    [SerializeField] private float respawnTime = 60f; // 1 минута
    [SerializeField] private GameObject blockPrefab;  // Префаб глыбы (перетащи сюда себя или префаб)
    
    [Header("🎲 Рандом (опционально)")]
    [SerializeField] private float positionVariance = 0.5f; // Разброс позиции при респауне
    [SerializeField] private bool randomRotation = false;   // Случайный поворот

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private bool _isDestroyed = false;

    private void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        
        // Если префаб не назначен — используем этот объект как префаб
        if (blockPrefab == null)
            blockPrefab = gameObject;
    }

    // 🔹 Вызывается из MineableBlock при уничтожении
    public void OnBlockDestroyed()
    {
        if (_isDestroyed) return;
        _isDestroyed = true;
        
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        Debug.Log($"⏳ Глыба исчезла. Респаун через {respawnTime} сек...");
        yield return new WaitForSeconds(respawnTime);

        // Создаём новую глыбу
        Vector3 spawnPos = _originalPosition;
        if (positionVariance > 0)
        {
            spawnPos += new Vector3(
                Random.Range(-positionVariance, positionVariance),
                Random.Range(-positionVariance, positionVariance),
                Random.Range(-positionVariance, positionVariance)
            );
        }

        Quaternion spawnRot = randomRotation ? Quaternion.Euler(0, Random.Range(0, 360), 0) : _originalRotation;

        GameObject newBlock = Instantiate(blockPrefab, spawnPos, spawnRot);
        Debug.Log($"✨ Глыба возрождена на позиции {spawnPos}");
    }
}