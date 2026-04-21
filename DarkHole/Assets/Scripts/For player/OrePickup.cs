using UnityEngine;

public class OrePickup : MonoBehaviour
{
    [Header("🪨 Тип руды")]
    [SerializeField] private string oreName = "Stone";
    [SerializeField] private int oreValue = 1;

    [Header("⚙️ Подбор")]
    [SerializeField] private float pickupRadius = 2.5f;      // Радиус, с которого можно подобрать
    [SerializeField] private LayerMask playerLayer;          // Слой игрока
    [SerializeField] private KeyCode collectKey = KeyCode.E; // Клавиша подбора

    [Header("🎨 UI (опционально)")]
    [SerializeField] private GameObject pickupPrompt; // Текст "Нажми E" (можно оставить пустым)

    private bool _isCollected = false;
    private bool _isPlayerInRange = false;
    private Transform _playerTransform;

    private void Start()
    {
        // Пытаемся найти игрока для точного расчёта дистанции (резервный метод)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
    }

    private void Update()
    {
        if (_isCollected) return;

        // 🔍 Проверяем, есть ли игрок в радиусе (2 способа для надёжности)
        bool playerDetected = false;
        
        // Способ 1: Physics.CheckSphere (твой исходный метод)
        if (Physics.CheckSphere(transform.position, pickupRadius, playerLayer))
        {
            playerDetected = true;
        }
        // Способ 2: Проверка по дистанции (если слои настроены неправильно)
        else if (_playerTransform != null && 
                 Vector3.Distance(transform.position, _playerTransform.position) <= pickupRadius)
        {
            playerDetected = true;
        }

        _isPlayerInRange = playerDetected;

        // Показываем/скрываем подсказку
        if (pickupPrompt != null)
            pickupPrompt.SetActive(_isPlayerInRange);

        // Если игрок в радиусе и нажал клавишу — подбираем
        if (_isPlayerInRange && Input.GetKeyDown(collectKey))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;

        // 🔔 Добавляем руду в инвентарь
        PlayerInventory.Instance?.AddOre(oreName, oreValue);
        Debug.Log($"✨ Подобрал: {oreName} x{oreValue}");

        // 🗑️ Удаляем предмет
        Destroy(gameObject);
    }

    // 🎨 Визуализация радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}