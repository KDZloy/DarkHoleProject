using UnityEngine;
using System.Collections;

public class PlayerMiner : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;      // 🔹 Перетащи сюда Animator НОВОЙ модели кирки
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private LayerMask blockLayer;   // 🔹 Слой глыб (Block / Ore)

    [Header("⚔️ Урон")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float penetration = 10f;

    [Header("⏱️ Тайминг и Анимация")]
    [SerializeField] private string mineTriggerName = "Mine"; // 🔹 Имя триггера в Animator новой модели
    [SerializeField] private float hitTiming = 0.25f;         // Момент удара в анимации
    [SerializeField] private float attackCooldown = 0.8f;

    private bool isMining = false;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    // 🔹 Публичный метод. Вызывается из WeaponManager или Input Actions по ЛКМ
    public void Swing()
    {
        if (isMining) return;
        StartCoroutine(MineSequence());
    }

    private IEnumerator MineSequence()
    {
        isMining = true;

        // 1. Запуск анимации
        if (animator != null)
        {
            animator.SetTrigger(mineTriggerName);
        }
        else
        {
            Debug.LogWarning("[Кирка] ⚠️ Animator не назначен! Проверь ссылку в Inspector.");
        }

        // 2. Ждём момент удара (синхронизация с анимацией)
        yield return new WaitForSeconds(hitTiming);

        // 3. Наносим урон
        ApplyDamage();

        // 4. Кулдаун
        yield return new WaitForSeconds(attackCooldown);

        isMining = false;
    }

    private void ApplyDamage()
    {
        if (playerCamera == null) return;

        // Визуализация луча в редакторе
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * miningRange, Color.green, 0.5f);
        
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, miningRange, blockLayer))
        {
            Debug.Log($"[Кирка] 🎯 Попал в: {hit.collider.name}");

            // Ищем скрипт на коллайдере или родителе
            MineableBlock block = hit.collider.GetComponent<MineableBlock>();
            if (block == null) block = hit.collider.GetComponentInParent<MineableBlock>();

            if (block != null)
            {
                block.TakeDamage(damage, penetration);
                Debug.Log($"[Кирка] ✅ Урон нанесён! Осталось HP: {block.GetCurrentHp()}");
            }
            else
            {
                Debug.LogWarning($"[Кирка] ⚠️ На {hit.collider.name} нет скрипта MineableBlock!");
            }
        }
        else
        {
            Debug.Log("[Кирка] 💨 Промах (луч не попал в слой Block или слишком далеко)");
        }
    }
    public void SetStats(float newDamage, float newPenetration, int newDurability)
    {
        damage = newDamage;
        penetration = newPenetration;
        // Если у тебя есть переменная прочности в кирке, обнови её тут:
        // durability = newDurability; 
        
        Debug.Log($"[Кирка] Обновлены статы: Урон={damage}, Пробитие={penetration}");
    }
}