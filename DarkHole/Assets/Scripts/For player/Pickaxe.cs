using UnityEngine;
using System.Collections.Generic;

public class Pickaxe : MonoBehaviour
{
    [Header("⚔️ Параметры")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float penetration = 10f;

    [Header("🎯 Зона удара")]
    [SerializeField] private Collider swingTrigger;
    [SerializeField] private float swingDuration = 0.35f;
    [SerializeField] private float swingCooldown = 0.5f;

    private bool _isSwinging = false;
    private float _swingTimer = 0f;
    private float _lastSwingTime = -1f;
    private HashSet<GameObject> _hitThisSwing = new HashSet<GameObject>();

    private void Start()
    {
        if (swingTrigger == null)
        {
            // Ищем первый дочерний коллайдер
            swingTrigger = GetComponentInChildren<Collider>();
        }

        if (swingTrigger != null)
        {
            swingTrigger.isTrigger = true;
            swingTrigger.enabled = false;
            Debug.Log("[Кирка] ✅ Зона удара найдена и подготовлена.");
        }
        else
        {
            Debug.LogError("[Кирка] ❌ Коллайдер не найден! Создай дочерний объект, добавь BoxCollider.");
        }

        // Unity НЕ вызывает триггеры, если на обоих объектах нет Rigidbody
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Debug.Log("[Кирка] 🔄 Добавлен кинематический Rigidbody.");
        }
    }

    // 📥 Вызывай из Input System / по ПКМ / по кнопке
    public void Swing()
    {
        if (Time.time - _lastSwingTime < swingCooldown || _isSwinging) return;

        _lastSwingTime = Time.time;
        _isSwinging = true;
        _swingTimer = swingDuration;
        _hitThisSwing.Clear();

        if (swingTrigger != null) swingTrigger.enabled = true;
        Debug.Log("[Кирка] 🥊 Замах АКТИВЕН. Коллайдер включён.");
    }

    private void Update()
{
    if (!_isSwinging) return;

    _swingTimer -= Time.deltaTime;
    if (_swingTimer <= 0f)
    {
        _isSwinging = false;
        if (swingTrigger != null) swingTrigger.enabled = false;
        return;
    }

    // Проверяем пересечение вручную каждый кадр
    Vector3 center = transform.position + transform.forward * 0.5f;
    Vector3 size = swingTrigger != null ? swingTrigger.bounds.size : Vector3.one;
    Collider[] hits = Physics.OverlapBox(center, size / 2f, transform.rotation);

    foreach (Collider col in hits)
    {
        if (_hitThisSwing.Contains(col.gameObject)) continue;

        MineableBlock block = col.GetComponent<MineableBlock>() ?? col.GetComponentInParent<MineableBlock>();
        if (block != null)
        {
            _hitThisSwing.Add(col.gameObject);
            block.TakeDamage(damage, penetration);
            Debug.Log($"[Кирка] ✅ OverlapBox попал в {col.name} | HP: {block.GetCurrentHp()}");
        }
    }
}

    // ⚠️ Точная сигнатура. Если перепутать на Collision, Unity промолчит
    private void OnTriggerEnter(Collider other)
    {
        if (!_isSwinging) return;
        if (_hitThisSwing.Contains(other.gameObject)) return;

        Debug.Log($"[Кирка] 📡 Триггер коснулся: {other.name} | Layer: {other.gameObject.layer}");

        MineableBlock block = other.GetComponent<MineableBlock>();
        if (block == null) block = other.GetComponentInParent<MineableBlock>();

        if (block != null)
        {
            _hitThisSwing.Add(other.gameObject);
            block.TakeDamage(damage, penetration);
            Debug.Log($"[Кирка] ✅ Урон нанесён! Осталось HP: {block.GetCurrentHp()}");
        }
        else
        {
            Debug.LogWarning($"[Кирка] ⚠️ На {other.name} нет скрипта MineableBlock!");
        }
    }

    // 🎨 Визуализация зоны удара в Scene-окне (выдели объект кирки)
    private void OnDrawGizmosSelected()
    {
        if (swingTrigger != null)
        {
            Gizmos.color = _isSwinging ? Color.red : Color.yellow;
            Bounds b = swingTrigger.bounds;
            if (swingTrigger is BoxCollider) Gizmos.DrawWireCube(b.center, b.size);
            else Gizmos.DrawWireSphere(b.center, b.extents.x);
        }
    }
}