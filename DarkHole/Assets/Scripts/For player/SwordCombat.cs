using UnityEngine;
using System.Collections;

public class SwordCombat : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    
    [Header("⚔️ Зона удара (Сфера)")]
    [SerializeField] private float hitOffset = 1.2f;      // Насколько вперед от камеры бьет меч
    [SerializeField] private float attackRadius = 1.5f;   // Радиус "шара" удара (увеличь, если мажешь)
    [SerializeField] private LayerMask enemyLayer;        // Слой, на котором стоит Зомби

    [Header("💥 Урон")]
    [SerializeField] private float baseDamage = 25f;

    [Header("⏱️ Тайминг")]
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private float hitTiming = 0.25f;     // Момент удара в анимации
    [SerializeField] private float attackCooldown = 0.7f;

    private bool isAttacking = false;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    public void Swing()
    {
        if (isAttacking) return;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // 1. Анимация
        if (animator != null)
            animator.SetTrigger(attackTriggerName);

        // 2. Ждем момента удара
        yield return new WaitForSeconds(hitTiming);

        // 3. Наносим урон
        ApplyDamage();

        // 4. Кулдаун
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void ApplyDamage()
    {
        if (playerCamera == null) return;

        // 🔹 Точка удара: позиция камеры + смотрим вперед
        Vector3 hitPoint = playerCamera.transform.position + playerCamera.transform.forward * hitOffset;

        // 🔹 ВИЗУАЛИЗАЦИЯ (Красная сфера покажет, куда ты бьешь)
        Debug.DrawRay(hitPoint, Vector3.up * 0.5f, Color.red, 0.5f);

        // 🔹 Ищем ВСЕХ врагов в радиусе сферы
        Collider[] hits = Physics.OverlapSphere(hitPoint, attackRadius, enemyLayer);

        // Чтобы не ударить одного зомби дважды за взмах
        System.Collections.Generic.HashSet<GameObject> hitEnemies = new System.Collections.Generic.HashSet<GameObject>();

        foreach (Collider col in hits)
        {
            ZombieAI zombie = col.GetComponent<ZombieAI>();
            if (zombie == null) zombie = col.GetComponentInParent<ZombieAI>();

            if (zombie != null && !hitEnemies.Contains(zombie.gameObject))
            {
                hitEnemies.Add(zombie.gameObject);
                zombie.TakeDamage(Mathf.RoundToInt(baseDamage));
                Debug.Log($"⚔️ Меч попал в {zombie.name} | Урон: {baseDamage}");
            }
        }

        if (hitEnemies.Count == 0)
        {
            Debug.Log("⚔️ Промах! (Враги вне радиуса удара)");
        }
    }

    // Рисуем сферу удара в редакторе, чтобы ты видел зону поражения
    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;
        Vector3 hitPoint = playerCamera.transform.position + playerCamera.transform.forward * hitOffset;
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Полупрозрачный красный
        Gizmos.DrawSphere(hitPoint, attackRadius);
    }
    public void SetDamage(float newDamage)
{
    baseDamage = newDamage;
    Debug.Log($"[Меч] Урон обновлен: {baseDamage}");
}
    public void SetStats(float newDamage, int newDurability)
    {
        baseDamage = newDamage;
        Debug.Log($"[Меч] Обновлен урон: {baseDamage}");
    }
}