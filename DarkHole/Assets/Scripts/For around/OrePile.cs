
using UnityEngine;

public class OrePile : MonoBehaviour
{
    [Header("Характеристики руды")]
    public float maxHealth = 100f;
    [Tooltip("Твёрдость от 0 до 1 (0 = мягкий, 1 = не ломается)")]
    public float hardness = 0.3f; // например, камень — 30% твёрдости

    private float currentHealth;

    [Header("Дроп")]
    public string[] dropItems = { "Камень", "Медь", "Железо", "Золото", "Алмаз", "Кобальт" };
    public float[] dropChances = { 40f, 25f, 15f, 10f, 6f, 4f }; // сумма = 100

    void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Наносит урон куче. Возвращает true, если уничтожена.
    /// </summary>
    public bool TakeDamage(float pickaxeDamage)
    {
        float effectiveDamage = pickaxeDamage * (1f - hardness);
        currentHealth -= effectiveDamage;

        Debug.Log($"{name}: получено {effectiveDamage:F1} урона. Осталось HP: {currentHealth:F1}");

        if (currentHealth <= 0)
        {
            DropLoot();
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private void DropLoot()
    {
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        for (int i = 0; i < dropChances.Length; i++)
        {
            cumulative += dropChances[i];
            if (roll < cumulative)
            {
                Debug.Log($"Выпал предмет: {dropItems[i]}");
                return;
            }
        }

        Debug.Log($"Выпал предмет: {dropItems[dropItems.Length - 1]}");
    }
}