using UnityEngine;
using TMPro; // Если используешь TextMeshPro
// using UnityEngine.UI; // Если используешь обычный Text (раскомментируй при необходимости)

public class PlayerHealth : MonoBehaviour
{
    [Header("❤️ Здоровье")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🛡️ Броня")]
    [SerializeField] private int armor = 0;

    [Header("📊 UI")]
    [SerializeField] private TextMeshProUGUI healthText; // Перетащи сюда текст здоровья

    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        Debug.Log($"👤 Игрок создан. Здоровье: {currentHealth}/{maxHealth}");
    }

    // 🔹 Получение урона
    public void TakeDamage(int amount)
    {
        // Броня уменьшает урон (50% от значения брони)
        int damageReduction = armor / 2;
        int finalDamage = Mathf.Max(1, amount - damageReduction);

        currentHealth -= finalDamage;
        Debug.Log($"👤 Игрок получил {finalDamage} урона. Здоровье: {currentHealth}/{maxHealth}");

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 🔹 Лечение
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"👤 Игрок вылечен на {amount}. Здоровье: {currentHealth}/{maxHealth}");
        UpdateUI();
    }

    // 🔹 Смерть
    private void Die()
    {
        currentHealth = 0;
        Debug.Log("💀 ИГРОК УМЕР! Перезапуск сцены...");
        UpdateUI();

        // Перезапуск сцены через 2 секунды
        Invoke("RestartScene", 2f);
        
        // Останавливаем игру
        Time.timeScale = 0f;
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    // 🔹 Публичные методы
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;

    // 🔹 Обновление UI
    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = $"❤️ {currentHealth}/{maxHealth}";
        }
    }
}