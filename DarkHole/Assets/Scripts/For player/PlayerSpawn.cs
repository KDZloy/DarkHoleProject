using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("🎯 Точки спавна")]
    [SerializeField] private Transform[] spawnPoints; // Массив точек спавна

    [Header(" Игрок")]
    [SerializeField] private GameObject playerPrefab; // Префаб игрока
    [SerializeField] private string defaultSpawnPoint = "Spawn"; // Точка по умолчанию

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        string spawnPointName = SpawnPointManager.Instance?.GetSpawnPoint();
        
        // Если точки нет в менеджере — используем дефолтную
        if (string.IsNullOrEmpty(spawnPointName))
            spawnPointName = defaultSpawnPoint;

        // Ищем точку спавна по имени
        Transform spawnPoint = null;
        
        foreach (var point in spawnPoints)
        {
            if (point.name == spawnPointName)
            {
                spawnPoint = point;
                break;
            }
        }

        // Если не нашли — берём первую точку
        if (spawnPoint == null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[0];
            Debug.LogWarning($"⚠️ Точка спавна '{spawnPointName}' не найдена! Используется '{spawnPoint.name}'");
        }
        else if (spawnPoint == null)
        {
            Debug.LogError("❌ Нет точек спавна на сцене!");
            return;
        }

        // 🔹 Создаём игрока
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"✅ Игрок заспавнен в точке: {spawnPoint.name}");
        }
        else
        {
            Debug.LogError("❌ PlayerPrefab не назначен!");
        }

        // 🔹 Очищаем точку спавна
        SpawnPointManager.Instance?.ClearSpawnPoint();
    }

    private void OnDrawGizmos()
    {
        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point.position, 0.5f);
                Gizmos.DrawLine(point.position, point.position + point.forward * 2);
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}