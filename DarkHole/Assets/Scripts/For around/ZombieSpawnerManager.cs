using UnityEngine;

public class ZombieSpawnerManager : MonoBehaviour
{
    public static ZombieSpawnerManager Instance { get; private set; }

    [Header("📈 Прогрессия")]
    [SerializeField] private int baseHealth = 100;  // Здоровье первого зомби
    [SerializeField] private int healthPerSpawn = 10; // Прибавка за каждого следующего

    // 🔹 Статический счётчик — сохраняется между сценами!
    private static int _totalZombiesSpawned = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Не удалять при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 Вызывать при спавне каждого зомби
    public int GetNextZombieHealth()
    {
        int health = baseHealth + (_totalZombiesSpawned * healthPerSpawn);
        _totalZombiesSpawned++;
        Debug.Log($"🧟 Зомби #{_totalZombiesSpawned} | Здоровье: {health}");
        return health;
    }

    // 🔹 Сброс прогрессии (для новой игры / теста)
    public static void ResetProgression()
    {
        _totalZombiesSpawned = 0;
        Debug.Log("🔄 Прогрессия зомби сброшена!");
    }

    // 🔹 Геттер для отладки
    public static int GetSpawnCount() => _totalZombiesSpawned;
}