using UnityEngine;

public class OrePickup : MonoBehaviour
{
    [Header("🪨 Тип руды")]
    [SerializeField] private string oreName = "Stone";
    [SerializeField] private int oreValue = 1;

    [Header("⚙️ Подбор")]
    [SerializeField] private float pickupRadius = 1.2f;      // 🔹 УМЕНЬШЕНО с 2.5f до 1.2f
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private KeyCode collectKey = KeyCode.E;

    [Header("🎨 UI (опционально)")]
    [SerializeField] private GameObject pickupPrompt;

    [Header("⏱️ Защита от массового подбора")]
    [SerializeField] private float collectCooldown = 0.2f;   // 🔹 Задержка между сборами

    // 🔹 СТАТИЧЕСКИЙ таймер — общий для ВСЕХ кусков руды!
    private static float _lastCollectTime = -10f;

    private bool _isCollected = false;
    private bool _isPlayerInRange = false;
    private Transform _playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
    }

    private void Update()
    {
        if (_isCollected) return;

        // 🔹 Глобальная проверка: прошло ли время с последнего сбора?
        if (Time.time - _lastCollectTime < collectCooldown)
            return;

        bool playerDetected = false;
        
        // Способ 1: Проверка по дистанции (надёжнее чем CheckSphere)
        if (_playerTransform != null && 
            Vector3.Distance(transform.position, _playerTransform.position) <= pickupRadius)
        {
            playerDetected = true;
        }

        _isPlayerInRange = playerDetected;

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

        // 🔹 Обновляем глобальный таймер сбора
        _lastCollectTime = Time.time;

        PlayerInventory.Instance?.AddOre(oreName, oreValue);
        Debug.Log($"✨ Подобрал: {oreName} x{oreValue}");

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}