using UnityEngine;

public class StonePile : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name}: осталось {currentHealth} HP");

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Или замените на эффект/дроп
        }
    }
}