using UnityEngine;

public class OrePickup : MonoBehaviour
{
    [Header("🪨 Тип руды")]
    [SerializeField] private string oreName = "Stone"; // Название для консоли
    [SerializeField] private int oreValue = 1; // Сколько штук добавляется (можно сделать руду по 2-3 шт)

    [Header("⚙️ Физика")]
    [SerializeField] private float pickupRadius = 1.5f; // Радиус подбора
    [SerializeField] private LayerMask playerLayer; // Слой игрока

    private bool _isCollected = false;

    private void Update()
    {
        // 🔍 Автоматический подбор при приближении игрока
        if (!_isCollected && Physics.CheckSphere(transform.position, pickupRadius, playerLayer))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;

        // 🔔 Сообщаем инвентарю о подборе
        PlayerInventory.Instance?.AddOre(oreName, oreValue);

        // 🎬 Визуал/звук (опционально)
        Debug.Log($"✨ Подобрал: {oreName} x{oreValue}");
        
        // 🗑️ Удаляем руду со сцены
        Destroy(gameObject);
    }

    // 🎨 Визуализация радиуса подбора в Scene-окне
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}