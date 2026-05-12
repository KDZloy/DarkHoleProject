using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("❤️ Здоровье")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🛡️ Броня")]
    [SerializeField] private int armor = 0;

    [Header("📊 UI - Текст")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("📊 UI - Полоска (Slider)")]
    [SerializeField] private Slider healthSlider;        // 🔹 НОВОЕ
    [SerializeField] private Image healthSliderFill;     // 🔹 НОВОЕ: для смены цвета
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        Debug.Log($"👤 Игрок создан. Здоровье: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        
        int damageReduction = armor / 2;
        int finalDamage = Mathf.Max(1, amount - damageReduction);
        
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth); // Не меньше 0
        
        Debug.Log($"👤 Игрок получил {finalDamage} урона. Здоровье: {currentHealth}/{maxHealth}");
        
        UpdateUI();
        
        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
        Debug.Log($"👤 Игрок вылечен на {amount}. Здоровье: {currentHealth}/{maxHealth}");
    }

    // 🔹 ГЛАВНЫЙ МЕТОД: Обновляет и текст, и полоску
    private void UpdateUI()
    {
        // Обновляем текст
        if (healthText != null)
        {
            healthText.text = $"❤️ {currentHealth}/{maxHealth}";
        }

        // Обновляем Slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 🔹 Меняем цвет полоски в зависимости от здоровья
        if (healthSliderFill != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthSliderFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
    }

    private void Die()
    {
        currentHealth = 0;
        Debug.Log("💀 ИГРОК УМЕР! Перезапуск сцены...");
        UpdateUI();
        Invoke(nameof(RestartScene), 2f);
        Time.timeScale = 0f;
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    // 🔹 Публичные геттеры для других скриптов
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
}