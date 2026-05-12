using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("❤️ Здоровье")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🛡️ Броня")]
    [SerializeField] private int armor = 0;

    [Header("📊 UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    public static PlayerHealth Instance { get; private set; }

    // 🔹 Запоминаем, где игрок появился в начале сцены
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Сохраняем стартовую позицию ДО любого перемещения
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
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
        currentHealth = Mathf.Max(0, currentHealth);
        
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

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"❤️ {currentHealth}/{maxHealth}";
    }

    private void Die()
    {
        currentHealth = 0;
        Debug.Log("💀 ИГРОК УМЕР! Потеря ресурсов и возрождение...");
        UpdateUI();

        // 1. Штраф: теряем всю руду и слитки
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.ClearResourcesOnly();

        // 2. Запускаем процесс смерти (пауза + таймер)
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Time.timeScale = 0f; // Пауза игры
        yield return new WaitForSecondsRealtime(2f); // Ждём 2 сек (игнорирует паузу)
        Respawn();
    }

    private void Respawn()
    {
        Time.timeScale = 1f; // Снимаем паузу
        currentHealth = maxHealth;
        UpdateUI();

        // 3. Возвращаем игрока в точку старта
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;

        // 4. Фикс застревания в полу/стенах при телепортации
        var cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.Move(Vector3.up * 0.3f);

        Debug.Log("🔄 Игрок возрождён в начальной позиции!");
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
}