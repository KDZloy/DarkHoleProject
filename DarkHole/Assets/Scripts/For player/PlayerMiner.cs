using UnityEngine;
using System.Collections;

public class PlayerMiner : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private LayerMask blockLayer; // 🔹 Слой глыб

    [Header("⚔️ Урон")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float penetration = 10f; // 🔹 Пробитие твёрдости

    [Header("⏱️ Тайминг")]
    [SerializeField] private float hitTiming = 0.25f; // 🔹 Когда в анимации наносится урон
    [SerializeField] private float hitCooldown = 0.8f;

    private const string ANIM_ATTACK = "Mine";
    private bool isMining = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMining)
        {
            StartCoroutine(MineSequence());
        }
    }

    private IEnumerator MineSequence()
    {
        isMining = true;
        animator?.SetTrigger(ANIM_ATTACK);

        // ⏱️ Ждём момент удара в анимации
        yield return new WaitForSeconds(hitTiming);
        
        ApplyDamage();

        // ⏱️ Кулдаун
        yield return new WaitForSeconds(hitCooldown);
        isMining = false;
    }

    private void ApplyDamage()
    {
        // 🔹 Визуализация луча в Scene-окне (только в редакторе)
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * miningRange, Color.green, 0.5f);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        if (Physics.Raycast(ray, out RaycastHit hit, miningRange, blockLayer))
        {
            Debug.Log($"[Игрок] 🎯 Попал в: {hit.collider.name}");

            // 🔹 Ищем скрипт на коллайдере или на родителе
            MineableBlock block = hit.collider.GetComponent<MineableBlock>();
            if (block == null) block = hit.collider.GetComponentInParent<MineableBlock>();

            if (block != null)
            {
                // 🔹 ВЫЗОВ С ДВУМЯ ПАРАМЕТРАМИ (damage, penetration)
                block.TakeDamage(damage, penetration);
                Debug.Log($"[Игрок] ✅ Урон нанесён! Осталось HP: {block.GetCurrentHp()}");
            }
            else
            {
                Debug.LogWarning($"[Игрок] ⚠️ На {hit.collider.name} нет скрипта MineableBlock!");
            }
        }
        else
        {
            Debug.Log("[Игрок] 💨 Промах (луч не попал в слой Block или слишком далеко)");
        }
    }
}