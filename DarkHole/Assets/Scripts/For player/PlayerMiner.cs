using UnityEngine;
using System.Collections;

public class PlayerMiner : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private LayerMask blockLayer;

    [Header("⚔️ Урон")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float penetration = 10f;

    [Header("⏱️ Тайминг и Анимация")]
    [SerializeField] private string mineTriggerName = "Mine";
    [SerializeField] private float hitTiming = 0.25f;
    [SerializeField] private float attackCooldown = 0.8f;

    private bool isMining = false;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    public void Swing()
    {
        if (isMining) return;
        StartCoroutine(MineSequence());
    }

    private IEnumerator MineSequence()
    {
        isMining = true;
        if (animator != null) animator.SetTrigger(mineTriggerName);
        else Debug.LogWarning("[Кирка] ⚠️ Animator не назначен!");

        yield return new WaitForSeconds(hitTiming);
        ApplyDamage();
        yield return new WaitForSeconds(attackCooldown);
        isMining = false;
    }

    private void ApplyDamage()
    {
        if (playerCamera == null) return;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * miningRange, Color.green, 0.5f);
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, miningRange, blockLayer))
        {
            Debug.Log($"[Кирка] 🎯 Попал в: {hit.collider.name}");
            MineableBlock block = hit.collider.GetComponent<MineableBlock>();
            if (block == null) block = hit.collider.GetComponentInParent<MineableBlock>();

            if (block != null)
            {
                // ✅ ВЫЗОВ С 2 ПАРАМЕТРАМИ (как в MineableBlock)
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

    // 🔹 ПУБЛИЧНЫЙ МЕТОД — через него меняем урон из WeaponManager
    public void SetStats(float newDamage, float newPenetration)
    {
        damage = newDamage;
        penetration = newPenetration;
        Debug.Log($"[Кирка] Обновлены статы: Урон={damage}, Пробитие={penetration}");
    }
}