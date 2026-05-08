using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    private string _nextSceneSpawnPoint = "";

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
    }

    // 🔹 Установить точку спавна для следующей сцены
    public void SetSpawnPoint(string sceneName, string spawnPointName)
    {
        _nextSceneSpawnPoint = spawnPointName;
        Debug.Log($"📍 Точка спавна установлена: {spawnPointName}");
    }

    // 🔹 Получить точку спавна
    public string GetSpawnPoint()
    {
        return _nextSceneSpawnPoint;
    }

    // 🔹 Очистить точку спавна (после использования)
    public void ClearSpawnPoint()
    {
        _nextSceneSpawnPoint = "";
    }
}