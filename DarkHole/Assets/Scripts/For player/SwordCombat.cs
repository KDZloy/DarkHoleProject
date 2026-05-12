using UnityEngine;
using System.Collections;

public class SwordCombat : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;          // Аниматор меча или руки
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private LayerMask enemyLayer;       // Слой Enemy

    [Header("⚔️ Параметры урона")]
    [SerializeField] private float baseDamage = 25f;

    [Header("⏱️ Тайминг и Анимация")]
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private float hitTiming = 0.25f;    // Момент касания в анимации
    [SerializeField] private float attackCooldown = 0.7f;

    private bool isAttacking = false;

    // 🔹 Вызывается из WeaponManager при нажатии ЛКМ
    public void Swing()
    {
        if (isAttacking) return;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // 1. Запуск анимации
        if (animator != null)
            animator.SetTrigger(attackTriggerName);

        // 2. Ждём момент удара (синхронизация с анимацией)
        yield return new WaitForSeconds(hitTiming);

        // 3. Наносим урон
        ApplyDamage();

        // 4. Кулдаун (защита от спама)
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    private void ApplyDamage()
    {
        // Луч из центра экрана (как в PlayerMiner)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
        {
            // Визуализация луча в редакторе
            Debug.DrawRay(ray.origin, ray.direction * attackRange, Color.red, 0.3f);

            // Ищем зомби на коллайдере или родителе
            ZombieAI zombie = hit.collider.GetComponent<ZombieAI>();
            if (zombie == null) zombie = hit.collider.GetComponentInParent<ZombieAI>();

            if (zombie != null)
            {
                zombie.TakeDamage(Mathf.RoundToInt(baseDamage));
                Debug.Log($"⚔️ Меч попал в {zombie.name} | Урон: {baseDamage}");
            }
            else
            {
                Debug.Log("⚔️ Попал в объект, но на нём нет ZombieAI");
            }
        }
        else
        {
            Debug.Log("⚔️ Меч: Промах");
        }
    }

    // 🔹 Для смены урона при крафте (например, алмазный меч = 40)
    public void SetDamage(float damage) => baseDamage = damage;
}