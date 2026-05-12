using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordCombat : MonoBehaviour
{
    [Header("🎯 Настройки")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float hitOffset = 1.2f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private LayerMask enemyLayer; // 🔹 ВАЖНО: назначь слой Enemy
    
    [Header("💥 Урон")]
    [SerializeField] private float baseDamage = 10f;
    
    [Header("⏱️ Тайминг")]
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private float hitTiming = 0.25f;
    [SerializeField] private float attackCooldown = 0.7f;

    private bool isAttacking = false;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    public void SetDamage(float newDamage)
    {
        baseDamage = newDamage;
    }

    public void Swing()
    {
        if (isAttacking) return;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        if (animator != null) animator.SetTrigger(attackTriggerName);
        
        yield return new WaitForSeconds(hitTiming);
        ApplyDamage();
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void ApplyDamage()
{
    // 🔹 ОТЛАДКА: покажи, что мы ищем
    Debug.Log($"[Меч] Ищем врагов в точке: {playerCamera.transform.position + playerCamera.transform.forward * hitOffset}");
    Debug.Log($"[Меч] Радиус: {attackRadius}, Слой: {enemyLayer.value}");
    
    if (playerCamera == null) 
    {
        Debug.LogError("[Меч] ❌ Camera не назначена!");
        return;
    }
    
    Vector3 hitPoint = playerCamera.transform.position + playerCamera.transform.forward * hitOffset;
    Collider[] hits = Physics.OverlapSphere(hitPoint, attackRadius, enemyLayer);
    
    Debug.Log($"[Меч] Найдено коллайдеров: {hits.Length}");
    
    HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    foreach (Collider col in hits)
    {
        Debug.Log($"[Меч] Коллайдер: {col.name} | Слой: {col.gameObject.layer}");
        
        ZombieAI zombie = col.GetComponent<ZombieAI>() ?? col.GetComponentInParent<ZombieAI>();
        if (zombie != null && !hitEnemies.Contains(zombie.gameObject))
        {
            hitEnemies.Add(zombie.gameObject);
            Debug.Log($"[Меч] ✅ Нашёл ZombieAI! Наношу урон: {Mathf.RoundToInt(baseDamage)}");
            zombie.TakeDamage(Mathf.RoundToInt(baseDamage));
        }
        else
        {
            Debug.LogWarning($"[Меч] ⚠️ На {col.name} нет ZombieAI!");
        }
    }
    
    if (hitEnemies.Count == 0)
        Debug.LogWarning("[Меч] ❌ Ни один зомби не пострадал!");
}

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;
        Vector3 hitPoint = playerCamera.transform.position + playerCamera.transform.forward * hitOffset;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(hitPoint, attackRadius);
    }
}